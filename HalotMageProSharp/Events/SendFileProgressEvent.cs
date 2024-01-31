namespace HalotMageProSharp.Events {
    public record SendFileProgressEvent {
        /// <summary>
        /// Key string of the file.
        /// </summary>
        public string Key { get; init; } = default!;

        /// <summary>
        /// Total size of the file.
        /// </summary>
        public int Size { get; init; }

        /// <summary>
        /// Received size of the file.
        /// </summary>
        public int Received { get; init; }

        /// <summary>
        /// 0 is success.
        /// </summary>
        public int ErrorCode { get; init; }
    }
}
