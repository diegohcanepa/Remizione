using Content.Builder;
using MonoGame.Framework.Content.Pipeline.Builder;

var builder = new Builder();

// We depend on the build arguments from the command line which
// are typically passed from your game project build commands.
if (args.Length > 0)
{
    builder.Run(args);
}
else
{
    builder.Run(new ContentBuilderParams { Mode = ContentBuilderMode.None });
}

return builder.FailedToBuild > 0 ? -1 : 0;

namespace Content.Builder
{
    /// <summary>
    /// Builder
    /// </summary>
    public sealed class Builder : ContentBuilder
    {
        public override IContentCollection GetContentCollection()
        {
            var content = new ContentCollection();

            // No content is imported by default.
            // Define your content collection rules here.
            //
            //   https://docs.monogame.net/articles/getting_started/content_pipeline/content_builder_project.html?#creating-your-contentcollection
            //
            // Examples:
            //
            // Import all content in the Assets folder using the default importer for their file type.
            // content.Include<WildcardRule>("*");
            //
            // Only copy content from the assets folder rather than build it with the pipeline.
            // content.IncludeCopy<WildcardRule>("*.json");
            // 
            // Exclude assets that match the pattern., only required overriding a default import behavior.
            // content.Exclude<WildcardRule>("Font/*.txt");
            //
            // Include a specific asset with processor parameters.
            // content.Include("Models/character.glb", new FbxImporter(),
            //    new MeshAnimatedModelProcessor()
            //    {
            //        Scale = 100.0f
            //    }
            // );

            content.Include<WildcardRule>("*");
            content.IncludeCopy<WildcardRule>("*.esl");
            content.IncludeCopy<WildcardRule>("*.json");
            content.Exclude<WildcardRule>("*.schema.json");
            content.Exclude<WildcardRule>("*.textrepository");
            content.Exclude<WildcardRule>("*.txt");
            content.IncludeCopy<WildcardRule>("*.xml");
            content.Exclude<WildcardRule>("Text/*.xml");
            content.IncludeCopy<WildcardRule>("*.lpkg");

            content.Include<WildcardRule>("Fonts/*.bmp", new LocalizedFontTextureImporter(),
                                                         new LocalizedFontTextureProcessor());

            return content;
        }
    }
}