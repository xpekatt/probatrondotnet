using System;
using System.Diagnostics;
using System.IO;
using Saxon.Api;
using net.sf.saxon.trans;

/**
 * Probatron.NET is a Schematron validation engine. 
 * This implementation uses the freely-available XSLT stylesheets plus Saxon 9.
 */
public class Probatron
{
    private const String ISO_SVRL = "iso_svrl.xsl";

    public Probatron()
    {
        
    }

    /**
     * @param name="instance" XML document to be validated
     * @param name="schema" XSLT stylesheet to compile the Schematron schema
     */
    public void run(String instance, String schema, String schemaCompilerXslt)
    {
        //compile the schema
        Debug.WriteLine("compiling schema '" + schema + "', instance=" + instance);
        String compiledSchema = TransformUtils.newTempFile();
        TransformUtils.transform(schema, schemaCompilerXslt, compiledSchema);
        Debug.WriteLine("schema compiled successfully");

        //TODO: check that the compiled schema is a valid XML document

        //validate the instance against the schema
        TransformUtils.transform(instance, compiledSchema );
        Debug.WriteLine("instance validated against schema successfully");
    }

    public static void Main(String[] args)
    {
        Debug.WriteLine("cwd=" + Environment.CurrentDirectory);

        String probatronHome = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().CodeBase );

        String schemaCompilerXslt = probatronHome + "/" + ISO_SVRL;

        Debug.WriteLine("iso_svrl location="+schemaCompilerXslt);

        Probatron p = new Probatron();

        if (args.Length == 2)
        {
            Debug.WriteLine("xml="+args[0]+" schema="+args[1]);
            p.run(args[0], args[1], schemaCompilerXslt);
            return;
        }
        else
            Console.WriteLine("usage: probatron.exe XML-document Schematron-schema");
    }

    
}
