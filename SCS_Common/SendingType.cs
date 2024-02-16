using System;

namespace SCS.Net.Common
{
    /// <summary>
    /// 발생한 <see cref="Exception"/> 처리기입니다.
    /// </summary>
    public delegate void ExceptionHandler(Exception ex);

    /// <summary>
    /// 전송 유형을 나타납니다.
    /// </summary>
    public enum SendingType
    {
        /// <summary>
        /// 클라이언트로 전달되는 메시지입니다.
        /// <see cref="SendingObj.TagData"/>로 <see cref="string"/>이 사용됩니다.
        /// </summary>
        SpreadMessageToClient,

        /// <summary>
        /// 서버로 전달되는 메시지 전달 요청입니다.
        /// <see cref="SendingObj.TagData"/>로 <see cref="SpreadMessageReqTag"/>가 사용됩니다.
        /// </summary>
        SpreadMessageReqToServer,

        /// <summary>
        /// 서버로 전송되는 캡쳐된 화면 이미지입니다.
        /// <see cref="SendingObj.TagData"/>로 <see cref="CapturedScreenTag"/>가 사용됩니다.
        /// </summary>
        CapturedScreenToServer,

        /// <summary>
        /// 서버로 전송되는 프로그램 종료 로그입니다.
        /// <see cref="SendingObj.TagData"/>로 <see cref="ProgramLogTag"/>가 사용됩니다.
        /// </summary>
        ProgramEndLogToServer,

        /// <summary>
        /// 서버로 전송되는 프로그램 제한 로그입니다.
        /// <see cref="SendingObj.TagData"/>로 <see cref="ProgramLogTag"/>가 사용됩니다.
        /// </summary>
        ProgramRestrictionLogToServer,

        /// <summary>
        /// 서버로 전송되는 URL 접속 로그입니다.
        /// <see cref="SendingObj.TagData"/>로 <see cref="URLAccessTag"/>가 사용됩니다.
        /// </summary>
        URLAccessLogToServer,

        /// <summary>
        /// 서버로 전송되는 URL 접속 제한 로그입니다.
        /// <see cref="SendingObj.TagData"/>로 <see cref="URLAccessTag"/>가 사용됩니다.
        /// </summary>
        URLRestrictionLogToServer,

        /// <summary>
        /// 서버로 전송되는 제한 설정 데이터 요청입니다.
        /// <see cref="SendingObj.TagData"/>는 지정되지 않습니다.
        /// </summary>
        RestrictionInitReqToServer,

        /// <summary>
        /// 클라이언트로 전송되는 제한 설정 데이터입니다.
        /// <see cref="SendingObj.TagData"/>로 <see cref="RestrictionInitTag"/>가 사용됩니다.
        /// </summary>
        RestrictionInitDataToClinet,

        /// <summary>
        /// 서버로 전송되는 연결 확인 요청입니다.
        /// <see cref="SendingObj.TagData"/>는 지정되지 않습니다.
        /// </summary>
        PingToServer,

        /// <summary>
        /// 서버에서 응답하는 연결 확인 요청입니다.
        /// <see cref="SendingObj.TagData"/>로 <see cref="bool"/>이 사용됩니다.
        /// true일 경우 비밀번호 일치, false일 경우 비밀번호 불일치를 나타냅니다.
        /// </summary>
        PingResponseToClient,

        /// <summary>
        /// 서버로 전송되는 보호해제 비밀번호 확인 요청입니다.
        /// <see cref="SendingObj.TagData"/>로 <see cref="string"/>이 사용됩니다.
        /// </summary>
        PasswordCheckToServer,

        /// <summary>
        /// 클라이언터로 전송되는 비밀번호 확인 요청 응답입니다.
        /// <see cref="SendingObj.TagData"/>로 <see cref="bool"/>이 사용됩니다.
        /// </summary>
        PasswordResponseToClient
    }
}
