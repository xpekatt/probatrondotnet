using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

//includes code taken from MSDN example
class ZipUtils
{
    public static String decompress( String filename )
    {
        Package package = Package.Open(filename);
        PackagePart word = package.GetPart( new Uri("/word/document.xml", UriKind.Relative));
        Stream stream = word.GetStream(FileMode.Open, FileAccess.Read);
        String target = TransformUtils.newTempFile();
        Stream outputStream = new FileStream(target, FileMode.Create);
        CopyStream( stream, outputStream );
        outputStream.Close();
        package.Close();

        return target;
    }

    private static void CopyStream(Stream source, Stream target)
    {
        const int bufSize = 0x1000;
        byte[] buf = new byte[bufSize];
        int bytesRead = 0;
        while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
            target.Write(buf, 0, bytesRead);
    }
}