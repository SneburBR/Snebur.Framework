# Description: PowerShell script to mark files as --assume-unchanged or --no-assume-unchanged

# Set the Git argument
# --assume-unchanged: For ignoring changes in the file
$gitArgument = "--assume-unchanged"

# Uncomment the following line to track changes again
#$gitArgument = "--no-assume-unchanged"

# Define the list of files
 

# Get all AssemblyInfo.cs files
$filesAssemblyInfo = Get-ChildItem -Path '..\src' -Recurse -Filter "AssemblyInfo.cs"

Write-Output "Found $($filesAssemblyInfo.Count) files."

# Get all tsconfig.json files
$filesTsConfig = Get-ChildItem -Path '..\src' -Recurse -Filter 'tsconfig.json'

# Combine both collections into a single array
$allFiles = $filesAssemblyInfo + $filesTsConfig

#Write count
Write-Output "Found $($filesAssemblyInfo.Count) files."


# Loop through each file and apply the Git argument
foreach ($file in $allFiles) {

    if (Test-Path $file) {
        git update-index $gitArgument $file
        Write-Output "Marked $file as $gitArgument."
    } else {
        Write-Output "Skipped ${file}: File does not exist."
    }
}
