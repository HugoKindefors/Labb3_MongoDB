using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Labb3.Models;

namespace Labb3.Services
{
	internal sealed class JsonStorageService
	{
		private static readonly JsonSerializerOptions Options = new()
		{
			WriteIndented = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};

		private static string AppFolder
			=> Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Labb3");

		private static string PacksPath => Path.Combine(AppFolder, "QuestionPacks.json");

		public async Task SaveAsync(IEnumerable<QuestionPack> packs)
		{
			Directory.CreateDirectory(AppFolder);
			await using var fs = new FileStream(PacksPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
			await JsonSerializer.SerializeAsync(fs, packs, Options).ConfigureAwait(false);
		}

		public async Task<IList<QuestionPack>> LoadAsync()
		{
			Directory.CreateDirectory(AppFolder);
			
			if (!File.Exists(PacksPath)) return new List<QuestionPack>();
			
			await using var fs = new FileStream(PacksPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
			var data = await JsonSerializer.DeserializeAsync<IList<QuestionPack>>(fs, Options).ConfigureAwait(false);
			return data ?? new List<QuestionPack>();
		}
	}
}

