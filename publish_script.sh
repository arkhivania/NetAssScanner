#!/bin/bash

sudo systemctl stop nailhang_silo
sudo systemctl stop nailhang_svn
sudo systemctl stop nailhang

dotnet publish -c Release ./Nailhang.Silo/
dotnet publish -c Release ./Nailhang.Web/
dotnet publish -c Release ./Nailhang.Svn/

cp prod_appsettings.json ./Nailhang.Web/bin/Release/netcoreapp2.0/publish/appsettings.json
cp prod_appsettings.json ./Nailhang.Silo/bin/Release/netcoreapp2.0/publish/appsettings.json
cp prod_appsettings.json ./Nailhang.Svn/bin/Release/netcoreapp2.0/publish/appsettings.json

sudo systemctl start nailhang_silo
sudo systemctl start nailhang_svn
sudo systemctl start nailhang


