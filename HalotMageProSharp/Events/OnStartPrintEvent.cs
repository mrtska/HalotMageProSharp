namespace HalotMageProSharp.Events {
    public record OnStartPrintEvent {
        /// <summary>
        /// Status of the start print request.
        /// STARTED, CHECK_FALSE.
        /// </summary>
        public string Status { get; init; } = default!;

        /// <summary>
        /// Filename of the file to print.
        /// </summary>
        public string Filename { get; init; } = default!;
    }
}
