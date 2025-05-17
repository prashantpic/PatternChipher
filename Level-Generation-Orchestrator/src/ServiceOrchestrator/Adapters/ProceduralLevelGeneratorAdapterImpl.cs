using PatternCipher.Services.Interfaces;
using PatternCipher.Services.Contracts; // For LevelGenerationParameters
using System.Threading.Tasks;
using System; // For ArgumentNullException
using UnityEngine; // For Debug.Log

// Assuming PatternCipher.Client and its components exist as per instructions
namespace PatternCipher.Client 
{
    // Placeholder for the actual generator from REPO-UNITY-CLIENT
    public class ProceduralLevelGenerator 
    {
        public Task<object> GenerateLayoutAsync(LevelGenerationParameters parameters)
        {
            Debug.Log($"[Client.ProceduralLevelGenerator.Stub] Generating layout with params: MinSize={parameters.GridMinSize}");
            // Simulate generation
            return Task.FromResult<object>(new { grid = "generated_grid_data_placeholder" });
        }
    }
}

namespace PatternCipher.Services.Adapters
{
    public class ProceduralLevelGeneratorAdapterImpl : IProceduralLevelGeneratorAdapter
    {
        private readonly Client.ProceduralLevelGenerator _generator; // As specified: "ProceduralLevelGenerator (from REPO-UNITY-CLIENT)"

        public ProceduralLevelGeneratorAdapterImpl(Client.ProceduralLevelGenerator generator)
        {
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        }

        public async Task<object> GenerateRawLevelAsync(LevelGenerationParameters generationParameters)
        {
            if (generationParameters == null) throw new ArgumentNullException(nameof(generationParameters));
            
            Debug.Log("[ProceduralLevelGeneratorAdapterImpl] Calling external ProceduralLevelGenerator.GenerateLayoutAsync");
            try
            {
                // Assuming the client generator has a method like GenerateLayoutAsync
                object rawLayout = await _generator.GenerateLayoutAsync(generationParameters);
                Debug.Log("[ProceduralLevelGeneratorAdapterImpl] Raw level layout received.");
                return rawLayout;
            }
            catch(Exception ex)
            {
                Debug.LogError($"[ProceduralLevelGeneratorAdapterImpl] Error calling external generator: {ex.Message}");
                throw; // Or wrap in a service-specific exception
            }
        }
    }
}