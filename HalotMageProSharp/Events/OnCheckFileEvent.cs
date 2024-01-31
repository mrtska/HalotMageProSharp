namespace HalotMageProSharp.Events {
    public record OnCheckFileEvent {
        /// <summary>
        /// Key string of the file.
        /// </summary>
        public string Key { get; init; } = default!;

        /// <summary>
        /// State of the upload.
        /// 0 is success.
        /// </summary>
        public int CheckState { get; init; }
    }
}
