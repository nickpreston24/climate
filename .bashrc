clone() {
  git clone  https://github.com/nickpreston24/"$1".git
}

find() {
	find . -iname "$1" -exec grep -inH "$2" {} \;
}

nureact(){ # new create react app with tailwind
 npx create-react-app "$1";
 npm install -D tailwindcss postcss autoprefixer;
 npx tailwindcss init -p ;
 
 cat > tailwind.config.js << EOL
/** @type {import('tailwindcss').Config} */

module.exports = {
content: [
   "./src/**/*.{js,jsx,ts,tsx}",
 ],
 theme: {
   extend: {},
 },
 plugins: [],
}
EOL;

cat > src/index.css << EOL

@tailwind base;
@tailwind components;
@tailwind utilities;

EOL;


}

nudi(){
	dotnet add package Microsoft.Extensions.DependencyInjection
}

nupage () {
    dotnet new page #--name "$1" --output Pages
}

nurazor(){
if [ -z "$1" ]; then
  echo "Error: Project name cannot be empty."
  exit 1
fi

dotnet new razor -n "$1";
cd "$1";

nunuget;
numechanic;
dap Hydro;
dap Serilog;
dap Serilog.Sinks.Console;
dap Serilog.Sinks.File;
dap Htmx;
dap Htmx.TagHelpers;
dap Dapper;
dap Lib.AspNetCore.ServerSentEvents;
dap MySql.Data;
dap NSpecifications;
dap Serilog.AspNetCore;
dap Sharprompt;

}

# new filebased generator project
nugen() {
 #dotnet new classlib -n "$1";
 dotnet add package  Microsoft.CodeAnalysis;
 dotnet add package  Microsoft.CodeAnalysis.Text;
}

nuinstaller(){

cat > install.sh << EOL

dotnet build;
dotnet pack;
dotnet tool install --add-source ./nupkg $1 --global | grep --invert-match warning --line-buffered

EOL
}


nucli(){
 numechanic;
 dotnet add package Microsoft.Extensions.DependencyInjection
 dotnet add package CodeMechanic.Shargs
 dotnet add package Sharprompt
 dotnet add package Spectre.Console
 dotnet add package Spectre.Console.Cli
 dotnet add Serilog
 dotnet add Serilog.Sinks.Console
 dotnet add Serilog.Sinks.File

}


nulogs(){
 dotnet add package Serilog;
 dotnet add package Serilog.Sinks.File;
 dotnet add package Serilog.Sinks.Console;
}

nunuget(){
cat > nuget.config << EOL
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>        
	<add key="Railway" value="https://portadev-production.up.railway.app/v3/index.json" />
    </packageSources>
    <packageSourceCredentials>
        <MyGet>
            <add key="Username" value="nickpreston17" />
            <add key="ClearTextPassword" value="************" />
        </MyGet>
    </packageSourceCredentials>
    <activePackageSource>
        <add key="All" value="(Aggregate source)" />
    </activePackageSource>
</configuration>

EOL
}



function cloneall(){
gh repo list "$1" --limit 1000 | while read -r repo _; do
  gh repo clone "$repo" "$repo" -- -q 2>/dev/null || (
    cd "$repo"
    # Handle case where local checkout is on a non-main/master branch
    # - ignore checkout errors because some repos may have zero commits, 
    # so no main or master
    git checkout -q main 2>/dev/null || true
    git checkout -q master 2>/dev/null || true
    git pull -q
  )
done

}

function steal(){
 
# Clone all github.com repositories for a specified user.

if [ $# -eq 0 ]
  then
    echo "Usage: $0 <user_name> "
    exit;
fi

USER=$1

# clone all repositories for user specified
for repo in `curl -s https://api.github.com/users/$USER/repos?per_page=1000 |grep git_url |awk '{print $2}'| sed 's/"\(.*\)",/\1/'`;do
git clone $repo;
done;

}

function build() {
	dotnet build --property WarningLevel=0 
}

function vsc(){
 code-insiders .
}


function pretty() {
 echo "running csharpier to format all files in current dir ... " 
 dotnet csharpier .
}

function rr(){
 railway run "$1"
}

function rebuild(){
sudo rm -rf obj bin
sudo dotnet clean
sudo dotnet build
sudo chmod +x obj
sudo chmod +x bin
}


function ipconfig(){
   nmcli dev show | grep 'IP4\.ADDRESS\|IP4.GATEWAY'
}

function sad() {
   dotnet sln add "$1";
}

function uninstall() {
  dotnet tool uninstall --global "$1";
}

function dap()
{
  dotnet add package "$1";
}

function phone(){
   dotnet watch run --urls http://192.168.1.173:3000 --no-build --non-interactive | grep --invert-match warning --line-buffered
}

function prod() {
echo "needs fixing"
 #  dotnet run -c Release .
}

function cli() {
    dotnet watch run --project . --debug  --no-build --non-interactive | grep --invert-match warning --line-buffered
}

function dev() {
   dotnet watch run --project . web --no-build --non-interactive | grep --invert-match warning --line-buffered
}

function install_dotnet_8(){
mkdir -p $HOME/dotnet && tar zxf ~/Downloads/dotnet-sdk-8.0.201-linux-musl-x64.tar.gz -C $HOME/dotnet 
export DOTNET_ROOT=$HOME/dotnet
export PATH=$PATH:$HOME/dotnet

mkdir -p $HOME/dotnet && tar zxf ~/Downloads/aspnetcore-runtime-8.0.201-linux-x64.tar.gz -C $HOME/dotnet
export DOTNET_ROOT=$HOME/dotnet
export PATH=$PATH:$HOME/dotnet

dotnet --info

}


function reinstall_dotnet_7() {

sudo snap remove dotnet-sdk
sudo apt remove 'dotnet*'
sudo apt remove 'aspnetcore*'
sudo apt remove 'netstandard*'
sudo rm /etc/apt/sources.list.d/microsoft-prod.list
sudo rm /etc/apt/sources.list.d/microsoft-prod.list.save
sudo apt update
sudo apt install dotnet7

}

function install_dotnet_7() {

#sudo apt install dotnet-host-7.0
#sudo apt install dotnet-sdk-7.0

echo "installing sdk ..."
mkdir -p $HOME/dotnet && tar zxf ~/Downloads/dotnet-sdk-7.0.406-linux-x64.tar.gz -C $HOME/dotnet
export DOTNET_ROOT=$HOME/dotnet
export PATH=$PATH:$HOME/dotnet

echo "installing aspnetcore runtime ... "
mkdir -p $HOME/dotnet && tar zxf ~/Downloads/aspnetcore-runtime-7.0.16-linux-x64.tar.gz -C $HOME/dotnet
export DOTNET_ROOT=$HOME/dotnet
export PATH=$PATH:$HOME/dotnet


#DOTNET_ROOT=/usr/lib/dotnet

dotnet --info
}



#DOTNET_FILE=~/Downloads/dotnet-sdk-7.0.406-linux-x64.tar.gz
#export DOTNET_ROOT=$(pwd)/.dotnet

#mkdir -p "$DOTNET_ROOT" && tar zxf "$DOTNET_FILE" -C "$DOTNET_ROOT"

#export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools

export PATH="$PATH:/opt/nvim-linux64/bin"




lazypack(){
	dotnet pack -p:PackageVersion="$1" -c Release -o ./nupkg
}


nubaget(){
# WARNING: This command contains a password.  Ideally, I wish to move this to a secret, somehow.
dotnet nuget push -s http://localhost:5555/v3/index.json -k Mintsharp24! ./nupkg/*
}


nupartial() {

cd Pages;
cat > "$1".cshtml <<EOF
@* @model object  *@

<div>
  <p>lorem ipsum</p>
</div> 

EOF
cd ..;

}

nugit(){
git init ;
cat > .gitignore << EOL
*.log
*.nupkg
.forklift/
obj/
bin/
nupkg/
.idea/
node_modules/
wwwroot/lib/
*.rest
*.env
EOL
}

nucontainer(){
   docker build -t $1
   docker run -it --rm --publish 3000:8080 $1 $1

}


lazynuget(){
  #nunuget
  nurelease "$1"
  nupush nupkg/
}

lazygit(){
	git add .
	git commit -m "$1"
	git push
}

nunuget(){
cat > nuget.config << EOL
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        
	<add key="Railway" value="https://portadev-production.up.railway.app/v3/index.json" />
    </packageSources>
    <packageSourceCredentials>
        <MyGet>
            <add key="Username" value="nickpreston17" />
            <add key="ClearTextPassword" value="************" />
        </MyGet>
    </packageSourceCredentials>
    <activePackageSource>
        <add key="All" value="(Aggregate source)" />
    </activePackageSource>
</configuration>
EOL

}

numechanic() {
	# !bin/bash
	## This installs essential CodeMechanic packages.
	dotnet add package CodeMechanic.Types
	dotnet add package CodeMechanic.Diagnostics
	dotnet add package CodeMechanic.FileSystem
	dotnet add package CodeMechanic.Embeds
	dotnet add package CodeMechanic.Reflection
	dotnet add package CodeMechanic.RegularExpressions


	dotnet restore;
	dotnet build;
}


nurelease() {

	dotnet pack -p:PackageVersion="$1" -c Release -o ./nupkg

}

nupush() {
	# dotnet nuget push "$1" -s https://www.myget.org/F/code-mechanic/api/v3/index.json --skip-duplicate --api-key a4001039-0f89-4952-b9bb-d84d87d8262c
         dotnet nuget push -s https://portadev-production.up.railway.app/v3/index.json "$1" -k $BAGETTER_API_KEY

}

nuke(){
dotnet clean;
rm -rf bin/;
rm -rf obj/;
rm -rf nupkg;
#dotnet clean
echo "removed all files from nupkg, bin and obj"
}

# bun
export BUN_INSTALL="$HOME/.bun"
export PATH=$BUN_INSTALL/bin:$PATH

# Turso
export PATH="/home/nick/.turso:$PATH"

# export PATH="$HOME/.yarn/bin:$HOME/.config/yarn/global/node_modules/.bin

## .env Variables

PUSHBULLET_PAT="o.CPHpTg384qrrJXr2EvgfFNNQvtQ0yPx6"; export PUSHBULLET_PAT
PB_REMOTE_URL="https://pocketbase-production-5ade.up.railway.app/"; export PB_REMOTE_URL
MYSQLDATABASE="railway"; export MYSQLDATABASE
MYSQLHOST="viaduct.proxy.rlwy.net"; export MYSQLHOST
MYSQLPASSWORD="TcqQAHOOlZBhuRcshGzVHUZwdXcjxjZz"; export MYSQLPASSWORD
MYSQLPORT="30137"; export MYSQLPORT
MYSQLUSER="root"; export MYSQLUSER
MYSQL_URL="mysql://root:TcqQAHOOlZBhuRcshGzVHUZwdXcjxjZz@viaduct.proxy.rlwy.net:30137/railway"; export MYSQL_URL
