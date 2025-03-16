# update contaoiner with latest packages
sudo apt update
sudo apt upgrade -y

#  Install mkcert need to for HTTPS used by Azurite
sudo apt install libnss3-tools -y

curl -s https://api.github.com/repos/FiloSottile/mkcert/releases/latest | grep browser_download_url | grep '\linux-amd64' | cut -d '"' -f 4 | wget -i -
sudo mv mkcert-v*-linux-amd64 /usr/bin/mkcert
sudo chmod +x /usr/bin/mkcert

mkcert -install
mkcert 127.0.0.1
mv 127.0.0.1.pem ./azurite/127.0.0.1.pem
mv 127.0.0.1-key.pem ./azurite/127.0.0.1-key.pem

#  Upate .NET SDK
sudo dotnet workload update

# generate a self-signed certificate for HTTPS
dotnet dev-certs https

# Restore the project dependencies
cd ./src
dotnet restore