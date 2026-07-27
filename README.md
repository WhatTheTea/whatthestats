# WhatTheStats

WhatTheStats is an yet another tool to write github stats to `README.md` files. I took it as a coding challenge and wanted to implement it by myself.

## Usage (dotnet cli)
```
Usage:
  cs project:
  dotnet run -- <USERNAME> [<README_PATH>] [<TOKEN>]

  executable:
  whatthestats <USERNAME> [<README_PATH>] [<TOKEN>]

Arguments:
  USERNAME      (Required) GitHub username.
  README_PATH   Path to the README file. [default: ./README.md]
  TOKEN         Personal access token for authentication.
```