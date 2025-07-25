using System.Reflection;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using MDXReForged;
using MDXReForged.MDX;

namespace MDXDebug
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var files = Directory.EnumerateFiles(@"C:\Users\Administrator\Downloads\cascview_en\x64\Work\war3.w3mod\_hd.w3mod\units\naga\heronagaseawitch", "*.mdx", SearchOption.AllDirectories);

            using (StreamWriter logFile = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "log.txt")))
            {
                foreach (var file in files)
                //Parallel.ForEach(files, file =>
                {
                    //logFile.WriteLine(file);
                    Model mdx = new Model(file);
                    VERS version = mdx.Get<VERS>();
                    MODL modl = mdx.Get<MODL>();
                    SEQS sequences = mdx.Get<SEQS>();
                    MTLS materials = mdx.Get<MTLS>();
                    TEXS textures = mdx.Get<TEXS>();
                    GEOS geosets = mdx.Get<GEOS>();
                    GEOA geosetanims = mdx.Get<GEOA>();
                    HELP helpers = mdx.Get<HELP>();
                    ATCH attachments = mdx.Get<ATCH>();
                    PIVT pivotpoints = mdx.Get<PIVT>();
                    CAMS cameras = mdx.Get<CAMS>();
                    EVTS events = mdx.Get<EVTS>();
                    CLID collisions = mdx.Get<CLID>();
                    GLBS globalsequences = mdx.Get<GLBS>();
                    PRE2 particleemitter2s = mdx.Get<PRE2>();
                    RIBB ribbonemitters = mdx.Get<RIBB>();
                    LITE lights = mdx.Get<LITE>();
                    TXAN textureanimations = mdx.Get<TXAN>();
                    BONE bones = mdx.Get<BONE>();
                    BPOS bindpositions = mdx.Get<BPOS>();
                    FAFX faceFXes = mdx.Get<FAFX>();
                    CORN particleEmitterPopcorns = mdx.Get<CORN>();

                    foreach (var mat in materials)
                    {
                        foreach (var lay in mat.Layers)
                        {
                            foreach (var tex in lay.Textures)
                            {
                                Console.WriteLine(textures[(int)tex.TextureId].Image);
                            }
                        }
                        Console.WriteLine();
                    }

                    var bone = bones[10];
                    foreach (var node in bone.RotationKeys.Nodes)
                    {
                        Console.WriteLine($"{node.Time}: {node.Value}");
                    }
                }
                ;
            }
            Console.Read();
        }
    }
}
