using MDXReForged.MDX;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace MDXReForged.Test
{
    [TestClass]
    public class Main
    {
        [TestMethod]
        public void BulkReadTest()
        {
            var files = Directory.EnumerateFiles(@"F:\refUnpacked\war3.w3mod", "*.mdx", SearchOption.AllDirectories);

            using (StreamWriter logFile = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "log.txt")))
            {
                foreach (var file in files)
                    //Parallel.ForEach(files, file =>
                    {
                    //logFile.WriteLine(file);
                    var mdx = new Model(file);

                    var version = mdx.GetVersionChunk();
                    var modelChunk = mdx.GetModelChunk();

                    var sequences = mdx.GetSequences();
                    var materials = mdx.GetMaterials();
                    var textures = mdx.GetTextures();
                    var geosets = mdx.GetGeosets();
                    var geosetanims = mdx.GetGeosetAnimations();
                    var helpers = mdx.GetHelpers();
                    var attachments = mdx.GetAttachments();
                    var pivotpoints = mdx.GetPivots();
                    var cameras = mdx.GetCameras();
                    var events = mdx.GetEvents();
                    var collisions = mdx.GetCollisionShapes();
                    var globalsequences = mdx.GetGlobalSequences();
                    var particleemitter2s = mdx.GetParticleEmitters2();
                    var ribbonemitters = mdx.GetRibbonEmitters();
                    var lights = mdx.GetLights();
                    var textureanimations = mdx.GetTextureAnims();
                    var bones = mdx.GetBones();
                    var bindpositions = mdx.GetBindPoses();
                    var faceFXes = mdx.GetFaceFX();
                    var particleEmitters = mdx.GetParticleEmitters();
                    var particleEmitterPopcorns = mdx.GetCornEmitters();
                }
                ;
            }
        }
    }
}
