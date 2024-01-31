namespace HalotMageProSharp.Events {
    public record SendFileEvent {
        /// <summary>
        /// Key string of the file.
        /// </summary>
        public string Key { get; init; } = default!;

        /// <summary>
        /// Filename of the file.
        /// </summary>
        public string Filename { get; init; } = default!;

        /// <summary>
        /// Size of the file.
        /// </summary>
        public int Size { get; init; }

        /// <summary>
        /// Compress the file.
        /// </summary>
        public bool Compress { get; init; }

        /// <summary>
        /// Offset of the file.
        /// </summary>
        public int Offset { get; init; }

        /// <summary>
        /// 0 is success.
        /// </summary>
        public int ErrorCode { get; init; }
    }
}
