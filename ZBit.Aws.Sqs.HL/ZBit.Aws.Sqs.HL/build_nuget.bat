nuget pack ZBit.Aws.Sqs.HL.csproj -Prop Configuration=Release -Symbols
pause
nuget push ZBit.Aws.Sqs.HL.1.0.2.0.nupkg
pause
#nuget push ZBit.Aws.Sqs.HL.1.0.2.0.symbols.nupkg
