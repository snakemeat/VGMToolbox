using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

using ICSharpCode.SharpZipLib.Checksums;
using VGMToolbox.util;

namespace VGMToolbox.format
{
    public class FujitsuEup : IFormat
    {
        private const string FORMAT_ABBREVIATION = "EUP";

        public static readonly string EUP_FILE_EXTENSION = ".EUP";
        public static readonly string FM_FILE_EXTENSION = ".FMB";
        public static readonly string PCM_FILE_EXTENSION = ".PMB";
        public static readonly string EXCEPTION_FM_MISSING = "FMB data for this file was not found.";
        public static readonly string EXCEPTION_PMB_MISSING = "PMB data for this file was not found.";

        
        public string FilePath { get; set; }
        public string TrackName { get; set; }
        public string ShortTrackName { get; set; }
        public string FmbFileName { get; set; }
        public string PmbFileName { get; set; }

        private byte[] trackNameBytes;
        private byte[] shortTrackNameBytes;
        private byte[] fmbFileNameBytes;
        private byte[] pmbFileNameBytes;

        private int pdxOffset;
        private int pdxLength;

        private int dataOffset;
        private int dataLength;

        Dictionary<string, string> tagHash = new Dictionary<string, string>();

        public FujitsuEup() { }

        public void Initialize(Stream pStream, string pFilePath)
        { 
            this.FilePath = pFilePath;

            using (FileStream fs = File.OpenRead(pFilePath)) 
            {
                this.TrackName = ParseFile.ReadAsciiString(fs, 0);
                this.ShortTrackName = ParseFile.ReadAsciiString(fs, 0x20);
                this.FmbFileName = ParseFile.ReadAsciiString(fs, 0x6E2, 8);
                this.PmbFileName = ParseFile.ReadAsciiString(fs, 0x6EA, 8);

                if (!String.IsNullOrEmpty(this.FmbFileName))
                {
                    this.FmbFileName += FM_FILE_EXTENSION;
                }
                
                if (!String.IsNullOrEmpty(this.PmbFileName))
                {
                    this.PmbFileName += PCM_FILE_EXTENSION;
                }
            }

            this.initializeTagHash();
        }
        private void initializeTagHash()
        {
            tagHash.Add("Title", this.TrackName);
            tagHash.Add("Short Name", this.ShortTrackName);
            
            if (!String.IsNullOrEmpty(this.FmbFileName))
            {
                tagHash.Add("FMB", this.FmbFileName);
            }

            if (!String.IsNullOrEmpty(this.PmbFileName))
            {
                tagHash.Add("PMB", this.PmbFileName);
            }
        }
        public byte[] GetAsciiSignature()
        {
            return null;
        }
        public string GetFileExtensions()
        {
            return EUP_FILE_EXTENSION;
        }
        public string GetFormatAbbreviation()
        {
            return FORMAT_ABBREVIATION;
        }
        public bool IsFileLibrary() { return false; }
        public bool HasMultipleFileExtensions()
        {
            return false;
        }
        public Dictionary<string, string> GetTagHash()
        {
            return this.tagHash;
        }
        public bool UsesLibraries() { return true; }
        public bool IsLibraryPresent()
        {
            bool ret = true;

            if (!String.IsNullOrEmpty(this.FmbFileName))
            {
                string searchFile =
                    Path.Combine(Path.GetDirectoryName(Path.GetFullPath(this.FilePath)), this.FmbFileName);

                if (!File.Exists(searchFile))
                {
                    ret = false;
                }
            }

            if (ret && !String.IsNullOrEmpty(this.PmbFileName))
            {
                string searchFile =
                    Path.Combine(Path.GetDirectoryName(Path.GetFullPath(this.FilePath)), this.PmbFileName);

                if (!File.Exists(searchFile))
                {
                    ret = false;
                }
            }

            return ret;
        }

        public void GetDatFileCrc32(ref Crc32 pChecksum)
        {
            throw new NotImplementedException();
        }
        public void GetDatFileChecksums(ref Crc32 pChecksum, ref CryptoStream pMd5CryptoStream,
            ref CryptoStream pSha1CryptoStream)
        {
            throw new NotImplementedException();
        }

    #region ISingleTagFormat Functions

    public string GetTagAsText() { return TrackName; }
        /*
        public void UpdateTag(string pNewValue)
        {
            string tempFilePath = Path.GetTempFileName();

            using (BinaryWriter bw =
                new BinaryWriter(File.Open(tempFilePath, FileMode.Open, FileAccess.ReadWrite)))
            {
                byte[] newTagBytes =
                    System.Text.Encoding.GetEncoding(VGMToolbox.util.ByteConversion.CodePageJapan).GetBytes(pNewValue);

                bw.Write(newTagBytes);
                bw.Write(TITLE_TERMINATOR);
                if (pdxBytes != null)
                {
                    bw.Write(pdxBytes);
                }
                bw.Write(PDX_TERMINATOR);
                bw.Write(dataBytes);
            }

            File.Delete(this.FilePath);
            File.Move(tempFilePath, this.FilePath);
        }
        */
        #endregion

    }


}
