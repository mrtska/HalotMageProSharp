# HalotMageProSharp

Simple automation library for Creality Halot Mage Pro 3D printer.

This is an unofficial library.
Please do not contact Creality regarding this library.

# Feature
- Send sliced file to the printer via Wi-Fi or Ethernet.
- Start, pause, stop, and set print parameters.
- Get printer status.

# Installation

## NuGet

```sh
Install-Package HalotMageProSharp
```

# Usage

1. Instanciate a `HalotMageProClient` object with the IP address of your printer.  
Password can be set in Settings > Print Settings > HALOT BOX > Password.

```cs
Client = new HalotMageProClient(Address, Password);
```


2. Prepare receiving data from the printer.

```cs
Client.OnConnected += (s, e) => {
};
Client.OnGetPrinterStatus += (s, e) => {
};
Client.OnTokenError += (s, e) => {
};
Client.OnVersionCheck += (s, e) => {
};
Client.OnSendFile += (s, e) => {
};
Client.OnSendFileProgress += (s, e) => {
};
Client.OnCheckFile += (s, e) => {
};
Client.OnStartPrint += (s, e) => {
};
Client.OnPausePrint += (s, e) => {
};
Client.OnStopPrint += (s, e) => {
};
Client.OnSetPrintParameter += (s, e) => {
};
Client.OnDisconnected += (s, e) => {
};
```

3. Connect to the printer.

```cs
Client.Connect();
```

For more details, please refer to the [interface](/HalotMageProSharp/IHalotMageProClient.cs).