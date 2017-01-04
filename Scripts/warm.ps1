while($true)
{
    try
    {        
        $web = New-Object Net.WebClient
        $res = $web.DownloadString("http://www.multivox.ru:21111/nailhang/Interfaces")
        $web.Dispose()

        $dt = Get-Date
        if($res.Length -gt 10)
        {            
            "Request complete: $dt"
        }
    }catch
    {
        "Request failed: $dt"
    }

    Start-Sleep -Seconds 10
}