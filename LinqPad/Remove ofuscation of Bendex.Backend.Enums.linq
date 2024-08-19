<Query Kind="Program" />

void Main()
{
	// Set the base directory to search for *.saproj files
	var baseDirectory = @"D:\Machines\";

	// Search for all *.saproj files recursively
	var saprojFiles = Directory.EnumerateFiles(baseDirectory, "*.saproj", SearchOption.AllDirectories);

	foreach (var file in saprojFiles)
	{
		ProcessSaprojFile(file);
	}
}

void ProcessSaprojFile(string filePath)
{
	try
	{
		// Load the XML document
		var doc = XDocument.Load(filePath);

		// Find the Assembly element with the specified AssemblyName
		var assemblyElement = doc
			.Descendants("SmartAssemblyProject")
			.Descendants("Configuration")
			.Descendants("Assemblies")
			.Descendants("Assembly")
			.FirstOrDefault(e => ((string)e.Attribute("AssemblyName")?.Value).StartsWith("Bendex.Backend.Enums"));

		if (assemblyElement != null)
		{
			// Find the Obfuscation element
			var obfuscationElement = assemblyElement.Descendants("Obfuscation").FirstOrDefault();

			if (obfuscationElement != null)
			{
				// Change the Obfuscate attribute value to "0"
				obfuscationElement.SetAttributeValue("Obfuscate", "0");

				// Save the document with 4 spaces indentation
				var settings = new XmlWriterSettings
				{
					Indent = true,
					IndentChars = new string(' ', 4),
					NewLineOnAttributes = false,
					OmitXmlDeclaration = true
				};

				using (var writer = XmlWriter.Create(filePath, settings))
				{
					doc.Save(writer);
				}

				Console.WriteLine($"Updated: {filePath}");
			}
			else
			{
				Console.WriteLine($"No Obfuscation element found in: {filePath}");
			}
		}
		else
		{
			Console.WriteLine($"No matching Assembly element found in: {filePath}");
		}
	}
	catch (Exception ex)
	{
		Console.WriteLine($"Error processing file {filePath}: {ex.Message}");
	}
}
