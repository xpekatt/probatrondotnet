using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Saxon.Api;
using System.IO;
using net.sf.saxon.trans;
using System.Xml;
using javax.xml.transform;


class TransformUtils
{
    private static Processor processor = new Processor();
    private static XsltCompiler compiler = processor.NewXsltCompiler();
    private static DocumentBuilder builder = processor.NewDocumentBuilder();


    /**
     * Runs an XSLT transform, serializing the result to a new temporary file.
     * 
     * @param name="src" the document to be transformed
     * @param name="stylesheet" the stylesheet to use
     */
    public static void transform(String src, String stylesheet, String tempFile )
    {
        XsltExecutable compiled = compileStylesheet(stylesheet);
        XsltTransformer transformer = compiled.Load();

        XdmNode doc = getSourceRoot(src);
        if (doc == null)
        {
            Console.WriteLine("error: null transform source root node");
            Environment.Exit(-1);
        }

        transformer.InitialContextNode = doc;
        Serializer serializer = new Serializer();
        serializer.SetOutputFile(tempFile);
        transformer.Run(serializer);
        serializer.Close();
    }

    /**
     * Runs an XSLT transform, serializing the result to standard output.
     * 
     * @param name="src" the document to be transformed
     * @param name="stylesheet" the stylesheet to use
     */
    public static void transform(String src, String stylesheet)
    {
        XsltExecutable compiled = compileStylesheet(stylesheet);
        XsltTransformer transformer = compiled.Load();

        XdmNode doc = getSourceRoot(src);
        if (doc == null)
        {
            Console.WriteLine("error: null transform source root node");
            Environment.Exit(-1);
        }

        transformer.InitialContextNode = doc;
        Serializer serializer = new Serializer();
        serializer.SetOutputStream(Console.OpenStandardOutput());
        transformer.Run(serializer);
        serializer.Close();
    }

    public static XsltExecutable compileStylesheet(String s)
    {
        Debug.WriteLine("compiling stylesheet " + s);
        XsltExecutable stylesheet = null;
        Uri uri = null;

        try
        {
            uri = new Uri(s);
        }
        catch (UriFormatException e)
        {
            Console.WriteLine("error compiling stylesheet '" + s + "': " + e.Message);
            Environment.Exit(0);
        }

        try
        {
            stylesheet = compiler.Compile(uri);
        }
        catch (TransformerConfigurationException e)
        {
            Console.WriteLine("error compiling stylesheet '" + s + "': " + e.Message);
            Environment.Exit(0);
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine("error compiling stylesheet: " + e.Message);
            Environment.Exit(0);
        }
        return stylesheet;
    }

    private static XdmNode getSourceRoot(String src)
    {
        Debug.WriteLine("getting source root node for " + src);

        Uri uri = null;
        try
        {
            XmlUrlResolver resolver = new XmlUrlResolver();
            uri = resolver.ResolveUri(null, src);   //null sets the current location as the resolution base
        }
        catch (UriFormatException e)
        {
            Console.WriteLine("error getting transformation source '" + uri + "': " + e.Message);
        }

        Debug.WriteLine("resolved uri=" + uri.AbsolutePath);

        XdmNode doc = null;
        try
        {
            doc = builder.Build(uri);
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine("error getting transformation source: " + e.Message);
        }
        catch (XPathException e)
        {
            Console.WriteLine("error getting transformation source: " + e.Message);
        }
        return doc;
    }

    public static String newTempFile()
    {
        try
        {
            return Path.GetTempFileName();
        }
        catch (IOException e)
        {
            Console.WriteLine("error creating temp file: " + e.Message);
        }
        return null;
    }
}
