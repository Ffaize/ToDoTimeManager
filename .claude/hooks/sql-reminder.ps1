$data = [Console]::In.ReadToEnd() | ConvertFrom-Json
$fp = $data.tool_input.file_path
if ($fp -match '\.sql$') {
    Write-Host 'REMINDER: Register this .sql file in ToDoTimeManager.DataBase.sqlproj'
}
