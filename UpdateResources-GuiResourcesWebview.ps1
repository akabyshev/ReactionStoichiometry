# this should've been a MSBuild task

Add-Type -AssemblyName System.Web

$resxFilePath = "D:\Solutions\ReactionStoichiometry\ReactionStoichiometry.GUI\ResourcesWebview.resx"
[xml]$resxXml = Get-Content -Path $resxFilePath

$mapping = @{
    "jsContent" = "D:\Solutions\ReactionStoichiometry\ReactionStoichiometry.JsonViewer\ReactionStoichiometry.JsonViewer.js"
    "cssContent" = "D:\Solutions\ReactionStoichiometry\ReactionStoichiometry.JsonViewer\ReactionStoichiometry.JsonViewer.css"
    #"htmlContent" = "D:\Solutions\ReactionStoichiometry\ReactionStoichiometry.JsonViewer\ReactionStoichiometry.JsonViewer.html"
    }

foreach ($entry in $mapping.GetEnumerator())
{
    $key = $entry.Key
    $filePath = $entry.Value
    $dataNode = $resxXml.SelectSingleNode("/root/data[@name=""$key""]/value")

    if ($dataNode -ne $null) {
        $fileContent = Get-Content -Path $filePath -Raw
        #$cdata = $resxXml.CreateCDataSection([System.Web.HttpUtility]::HtmlEncode($filecontent))
        #$dataNode.RemoveAll()
        #$dataNode.AppendChild($cdata) | Out-Null
        $dataNode.InnerText = $fileContent
    } else {
        Write-Host "Node for $key not found."
    }
}

$resxXml.Save($resxFilePath)