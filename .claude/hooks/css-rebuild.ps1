$data = [Console]::In.ReadToEnd() | ConvertFrom-Json
$fp = $data.tool_input.file_path
if ($fp -match '\.css$' -and $fp -notmatch 'site\.min\.css') {
    Set-Location 'D:\Projects\ToDoTimeManager\ToDoTimeManager.WebUI'
    npx gulp build 2>&1 | Select-Object -Last 5
}
