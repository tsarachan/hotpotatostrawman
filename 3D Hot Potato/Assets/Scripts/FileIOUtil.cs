using UnityEngine;
using System.Collections;
using System.IO;
using SimpleJSON;

public class FileIOUtil {

	public static void WriteJsonToFile(string fileName, JSONClass json)
	{
		string output = json.ToString();

		WriteStringToFile(fileName, output, false);
	}

	public static JSONNode ReadJsonFromFile(string fileName)
	{
		if (Path.GetExtension(fileName) != ".json")
		{
			return null;
		}

		string input = ReadStringFromFile(fileName);

		JSONNode converted = JSONClass.Parse(input);

		return converted;
	}

	/// <summary>
	/// Write a string to a file
	/// </summary>
	/// <param name="fileName">File to write to.</param>
	/// <param name="content">String to write.</param>
	/// <param name="append">If set to <c>true</c>, append. If set to <c>false<c>, overwrite file.</param>
	public static void WriteStringToFile(string fileName, string content, bool append)
	{
		StreamWriter sw = new StreamWriter(fileName, append);

		sw.WriteLine(content);

		sw.Close();
	}

	/// <summary>
	/// Reads a string from a file.
	/// </summary>
	/// <returns>The string from file. If file does not exist, returns null.</returns>
	/// <param name="fileName">Name of file to read from.</param>
	public static string ReadStringFromFile(string fileName)
	{
		if (File.Exists(fileName))
		{
			StreamReader sr = new StreamReader(fileName);

			string input = sr.ReadToEnd();

			sr.Close();

			return input;
		}
		else
		{
			Debug.Log("Warning: attempted to read from \\" + fileName + ", file does not exist");
			return null;
		}
	}

	/// <summary>
	/// Creates an array of the items in a string, separating by a character provided as an argument.
	/// </summary>
	/// <returns>A string array.</returns>
	/// <param name="fileName">Name of file to read from.</param>
	/// <param name="separator">The character to use as a separator.</param>
	public static string[] SplitStringArrayFromFile(string filename, char separator)
	{
		string unsplitString = FileIOUtil.ReadStringFromFile(filename);
		
		return unsplitString.Split(new char[] { separator });
	}

	/// <summary>
	/// Turns a JSON node into a 1-dimensional string.
	/// </summary>
	/// <returns>The 1D string.</returns>
	/// <param name="retrieveInfo">Name of the JSON node to read.</param>
	/// <param name="filePath">File path.</param>
	/// <param name="fileName">File name.</param>
	public static string GetFromFile(string retrieveInfo, string filePath, string fileName)
	{
		JSONNode jsonLevel = FileIOUtil.ReadJsonFromFile(Application.dataPath + filePath + fileName);
		string readString = jsonLevel[retrieveInfo].ToString();
		readString = readString.Substring(1); //discard initial quotation mark that is part of JSON formatting
		readString = readString.TrimEnd('"');
		return readString;
	}
}
