using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Uplink.Redshirt
{
    public class Redshirt
    {
        private const int BufferSize = 16384;
        private const string Marker = "REDSHIRT\x0";
        private const string Marker2 = "REDSHRT2\x0";
        private const int SizeMarker = 9;

        private int ComputeChecksum(Stream input, byte[] hashBuffer)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                long currentPos = input.Position;
                input.Seek(0, SeekOrigin.Begin);

                byte[] buffer = new byte[BufferSize];
                int bytesRead;
                while ((bytesRead = input.Read(buffer, 0, BufferSize)) > 0)
                {
                    sha256.TransformBlock(buffer, 0, bytesRead, buffer, 0);
                }
                sha256.TransformFinalBlock(buffer, 0, 0);

                input.Seek(currentPos, SeekOrigin.Begin);
                Array.Copy(sha256.Hash, hashBuffer, hashBuffer.Length);
                return hashBuffer.Length;
            }
        }

        private void DecryptBuffer(byte[] buffer, int length)
        {
            for (int i = 0; i < length; i++)
            {
                buffer[i] -= 128;
            }
        }

        private void EncryptBuffer(byte[] buffer, int length)
        {
            for (int i = 0; i < length; i++)
            {
                buffer[i] += 128;
            }
        }

        private bool FilterStream(Stream input, Stream output, Action<byte[], int> filterFunc)
        {
            byte[] buffer = new byte[BufferSize];
            int bytesRead;

            while ((bytesRead = input.Read(buffer, 0, BufferSize)) > 0)
            {
                filterFunc(buffer, bytesRead);
                output.Write(buffer, 0, bytesRead);
            }

            return true;
        }

        private int HashResultSize()
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.HashSize / 8;
            }
        }

        private bool ReadHeader(Stream input)
        {
            byte[] markerBuffer = new byte[SizeMarker];
            if (input.Read(markerBuffer, 0, SizeMarker) != SizeMarker)
            {
                return false;
            }

            string marker = Encoding.ASCII.GetString(markerBuffer);
            if (marker == Marker2)
            {
                int hashSize = HashResultSize();
                byte[] hashBuffer = new byte[hashSize];
                return input.Read(hashBuffer, 0, hashSize) == hashSize;
            }
            else if (marker == Marker)
            {
                return true;
            }

            return false;
        }

        private bool WriteChecksum(Stream output)
        {
            long currentPos = output.Position;
            output.Seek(SizeMarker + HashResultSize(), SeekOrigin.Begin);

            byte[] hashBuffer = new byte[HashResultSize()];
            if (ComputeChecksum(output, hashBuffer) != hashBuffer.Length)
            {
                return false;
            }

            output.Seek(SizeMarker, SeekOrigin.Begin);
            output.Write(hashBuffer, 0, hashBuffer.Length);
            return true;
        }

        private bool WriteHeader(Stream output)
        {
            byte[] markerBuffer = Encoding.ASCII.GetBytes(Marker2);
            output.Write(markerBuffer, 0, SizeMarker);

            int hashSize = HashResultSize();
            byte[] hashBuffer = new byte[hashSize];
            Array.Clear(hashBuffer, 0, hashBuffer.Length);
            output.Write(hashBuffer, 0, hashBuffer.Length);
            return true;
        }

        public byte[] Decrypt(byte[] dataIn)
        {
            using (MemoryStream input = new MemoryStream(dataIn))
            using (MemoryStream output = new MemoryStream())
            {
                if (!ReadHeader(input))
                {
                    throw new InvalidOperationException("Failed to read header.");
                }

                if (!FilterStream(input, output, DecryptBuffer))
                {
                    throw new InvalidOperationException("Failed to decrypt data.");
                }

                return output.ToArray();
            }
        }

        public byte[] Encrypt(byte[] dataIn)
        {
            using (MemoryStream input = new MemoryStream(dataIn))
            using (MemoryStream output = new MemoryStream())
            {
                if (!WriteHeader(output))
                {
                    throw new InvalidOperationException("Failed to write header.");
                }

                if (!FilterStream(input, output, EncryptBuffer))
                {
                    throw new InvalidOperationException("Failed to encrypt data.");
                }

                if (!WriteChecksum(output))
                {
                    throw new InvalidOperationException("Failed to write checksum.");
                }

                return output.ToArray();
            }
        }

        public bool IsRedshirted(byte[] dataIn)
        {
            using (MemoryStream input = new MemoryStream(dataIn))
            {
                return ReadHeader(input);
            }
        }
    }
}