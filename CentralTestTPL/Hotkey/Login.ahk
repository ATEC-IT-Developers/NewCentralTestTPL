WinWait, Please login, , 10         ; Waits up to 10 seconds for the window

if ErrorLevel
{
    MsgBox, 16, Error, Window not found.
}
else
{
    WinActivate, Please login
    WinWaitActive, Please login
    Sleep, 1000
    ControlFocus, Edit1, Please login
    Sleep, 500
    inputFile := "C:\ITfolder\TPL\AutoFillDetails.txt"
    FileRead, fileContent, %inputFile%
    fileContent := StrReplace(fileContent, "`r", "")
    StringSplit, param, fileContent,`n
    Sleep, 500
    Send, %param5%
    Sleep, 500
    Send, {TAB}
    Sleep, 500
    Send, %param6%
    Sleep, 500
    Send, {TAB}
    Sleep, 500
    Send, {Enter}
    Sleep, 5000
    Run, "C:\ITfolder\TPL\LoadTestProg.exe"
}