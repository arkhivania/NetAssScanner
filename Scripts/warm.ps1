while($true)
{
    Invoke-WebRequest -Uri http://www.multivox.ru:21111/nailhang/Interfaces
    Start-Sleep -Seconds 10
}