namespace HalotMageProSharp.Events {
    public record OnCommandEvent {
        /// <summary>
        /// Command result.
        /// CMD_FAIL, CMD_SUCCES.
        /// </summary>
        public string Status { get; init; } = default!;
    }
}
