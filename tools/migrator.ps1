param (
    [string][Parameter(Mandatory = $true)]$Name
)

function AddMigration([string]$projectFolder) {
    dotnet ef migrations add $Name --startup-project ..\ALo.Addresses.FiasUpdater\
}