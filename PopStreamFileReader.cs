//#if NET_4_6	//	gr: not working?
#if true
//#define USE_MEMORY_MAPPED_FILE
#define USE_FILE_HANDLE
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pop;

#if USE_MEMORY_MAPPED_FILE
//	.net4 required
using System.IO.MemoryMappedFiles;
#endif

namespace Pop
{
	[System.Serializable]
	public class UnityEvent_FileChunk : UnityEngine.Events.UnityEvent<byte[]> { };
}


public class PopStreamFileReader : MonoBehaviour
{
	public string Filename;
	public Pop.UnityEvent_FileChunk OnFileChunkRead;
	[Range(0,50)]
	public int ReadMbSec = 5;
	public int ReadKbSec { get { return ReadMbSec * 1024; } }
	public int ReadBytesSec { get { return ReadKbSec * 1024; } }
	public int ReadBytesFrame { get { return ReadBytesSec / 60; } }	//	yeah, I know, not always 60fps...

	int BytesRead = 0;

	//	gr: make these an option of which to use
#if USE_MEMORY_MAPPED_FILE
	MemoryMappedFile File;
	MemoryMappedViewAccessor FileView;
	long FileSize;			//	the file view capcacity is bigger than the file size (page aligned) so we need to know the proper size
#elif USE_FILE_HANDLE
	System.IO.FileStream File;
#else
	byte[] FileBytes;
#endif

	//	lambda to read a chunk
	//	if this doesn't exist, we haven't started
	System.Func<long, long, byte[]> ReadFunctor;

	void OnDisable()
	{
		//	stop read, close file etc
		ReadFunctor = null;
	}

	void Update()
	{
		//	start read
		if (ReadFunctor==null)
			ReadFunctor = GetReadFileFunction();

		//	read next chunk
		var Start = this.BytesRead;
		var Size = ReadBytesFrame;
		var Chunk = ReadFunctor(Start, Size);
		if (Chunk.Length > 0)
		{
			this.BytesRead += Chunk.Length;
			OnFileChunkRead.Invoke(Chunk);
		}
		else
		{
			//	didnt read, eof? work out what user wants here
		}
	}

	public System.Func<long, long, byte[]> GetReadFileFunction()
	{
		//	if not absolute path
		if (!System.IO.File.Exists(Filename))
		{
			//	try a file in the project assets
			var AssetFilename = "Assets/" + Filename;
			if (System.IO.File.Exists(AssetFilename))
				Filename = AssetFilename;

			//	try in streaming assets
			var StreamingAssetsFilename = Application.streamingAssetsPath + "/" + Filename;
			if (System.IO.File.Exists(StreamingAssetsFilename))
				Filename = StreamingAssetsFilename;
		}

		//	still not found
		if (!System.IO.File.Exists(Filename))
			throw new System.Exception("File missing: " + Filename);

		Debug.Log("FileReader opening file " + Filename + " (length = " + Filename.Length + ")");

#if USE_MEMORY_MAPPED_FILE
		Debug.Log("Creating Memory mapped file");
		File = MemoryMappedFile.CreateFromFile(Filename,System.IO.FileMode.Open);
		Debug.Log("Memory mapped file = "+ File);
		FileView = File.CreateViewAccessor();
		Debug.Log("Memory mapped FileView = " + FileView);
		FileSize = new System.IO.FileInfo(Filename).Length;
		Debug.Log("Memory mapped FileSize = " + FileSize);

		System.Func<long, long, byte[]> ReadFileBytes = (long Position, long Size) =>
		{
			var Data = new byte[Size];
			//	gr: [on OSX at least] you can read past the file size, (but within capacity)
			//		this doesn't error, but does fill the bytes with zeros.
			var BytesRead = FileView.ReadArray(Position, Data, 0, (int)Size);
			if (BytesRead != Size)
				throw new System.Exception("Memory mapped file only read " + BytesRead + "/" + Size + " bytes");
			return Data;
		};

#elif USE_FILE_HANDLE
		File = System.IO.File.OpenRead(Filename);
		System.Func<long, long, byte[]> ReadFileBytes = (long Position, long Size) =>
		{
			var Data = new byte[Size];
			//	gr: this will succeed, even if passed EOF
			var NewPos = File.Seek(Position, System.IO.SeekOrigin.Begin);
			if (NewPos != Position)
				throw new System.Exception("Seeked to " + Position + " but stream is at " + NewPos);
			var BytesRead = File.Read(Data, 0, (int)Size);
			//	probably EOF, but no errors
			if (BytesRead != Size)
				return Data.SubArray(0, BytesRead);
			//	throw new System.Exception("FileStream only read " + BytesRead + "/" + Size + " bytes");
			return Data;
		};
#else
		FileBytes = System.IO.File.ReadAllBytes(Filename);
		System.Func<long, long, byte[]> ReadFileBytes = (long Position, long Size) =>
		{
			return FileBytes.SubArray(Position, Size);
		};
#endif

		return ReadFileBytes;
	}

}
