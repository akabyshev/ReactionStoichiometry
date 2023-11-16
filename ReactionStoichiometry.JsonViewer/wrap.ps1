$jsonContent = Get-Content -Raw -Path "D:\Solutions\ReactionStoichiometry\batchdata\Json-BalancerGeneralized.txt" -Encoding utf8
$wrappedContent = "const jsonData = `n$jsonContent"
$wrappedContent | Out-File -FilePath "D:\Solutions\ReactionStoichiometry\ReactionStoichiometry.JsonViewer\wrapped_json.js" -Encoding utf8