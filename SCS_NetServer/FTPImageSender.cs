using System;
using System.Collections;
using System.Drawing;
using System.IO;
using Tamir.SharpSsh.jsch;
using Tamir.Streams;

namespace SCS.Net.Server
{
    /// <summary>
    /// SFTP를 통해 이미지를 전송합니다.
    /// </summary>
    public class FTPImageSender : IDisposable
    {
        private string m_host;
        private string m_userName;
        private string m_keyFilePath;
        private string m_remotePath;

        private Session session;
        private ChannelSftp channel;

        /// <summary>
        /// SFTP 서버 정보를 설정합니다.
        /// </summary>
        public void SetServerInfo(string host, string userName, string keyFilePath, string remotePath)
        {
            m_host = host;
            m_userName = userName;
            m_keyFilePath = keyFilePath;
            m_remotePath = remotePath;
        }

        /// <summary>
        /// 서버와 연결합니다.
        /// </summary>
        private bool TryConnect()
        {
            if (m_host == null || m_userName == null || m_keyFilePath == null)
                throw new NotImplementedException();

            try
            {
                Program.m_Logger.WriteLog("[FTPImageSender] Connecting SFTP Server");

                JSch jsch = new JSch();

                //Add the identity file to JSch
                jsch.addIdentity(m_keyFilePath);

                //Create a new SSH session
                session = jsch.getSession(m_userName, m_host, 22);

                // 세션과 관련된 정보를 설정
                Hashtable p = new Hashtable();

                // 호스트 정보를 검사하지 않음
                p.Add("StrictHostKeyChecking", "no");
                session.setConfig(p);

                //Connect to remote SSH server
                session.connect();

                //Open a new Shell channel on the SSH session
                channel = (ChannelSftp)session.openChannel("sftp");

                //Connect the channel
                channel.connect();
                channel.cd(m_remotePath);
                Program.m_Logger.WriteLog("[FTPImageSender] connected to SFTP completed");
                return true;
            }
            catch (SftpException ex)
            {
                Program.m_Logger.WriteLog("[FTPImageSender] SftpException while TryConnect(): " + ex.message);
                Disconnect();
                return false;
            }
            catch (Exception ex)
            {
                Program.m_Logger.WriteLog("[FTPImageSender] Exception while TryConnect(): " + ex.Message);
                Disconnect();
                return false;
            }
        }

        public void UploadImage(Image image, string imageName)
        {
            // 연결되지 않았을 경우 연결 시도
            if (session == null || !session.isConnected())
                if (!TryConnect()) // 연결에 실패할 경우 바로 리턴됨
                    return;

            MemoryStream memStream = null;
            InputStreamWrapper wrapperStream = null;

            try
            {
                memStream = new MemoryStream();
                image.Save(memStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                memStream.Position = 0;

                wrapperStream = new InputStreamWrapper(memStream);
                channel.put(wrapperStream, imageName, ChannelSftp.OVERWRITE);
                Program.m_Logger.WriteLog("[FTPImageSender] File upload completed: " + imageName);
            }
            catch (SftpException ex)
            {
                Program.m_Logger.WriteLog("[FTPImageSender] SftpException while UploadImage(): " + ex.message);
            }
            catch (Exception ex)
            {
                Program.m_Logger.WriteLog("[FTPImageSender] Exception while UploadImage(): " + ex.Message);
            }
            finally
            {
                if (wrapperStream != null)
                    wrapperStream.close();
                if (memStream != null)
                    memStream.Close();
            }
        }

        public void Disconnect()
        {
            if (channel != null)
            {
                channel.disconnect();
                channel = null;
            }

            if (session != null)
            {
                session.disconnect();
                session = null;
            }

            Program.m_Logger.WriteLog("[FTPImageSender] Disconnected to FTP Server");
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
