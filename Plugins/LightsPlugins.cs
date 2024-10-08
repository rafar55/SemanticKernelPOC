using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;

namespace SemanticKernel.Plugins;

public class LightsPlugin
{
    // Mock data for the lights
    private readonly List<LightModel> lights = new()
    {
        new LightModel { Id = 1, Name = "Table Lamp", IsOn = false },
        new LightModel { Id = 2, Name = "Porch light", IsOn = false },
        new LightModel { Id = 3, Name = "Chandelier", IsOn = true }
    };

    [KernelFunction("get_lights")]
    [Description("Gets a list of lights and their current state")]
    [return: Description("An array of lights")]
    public Task<List<LightModel>> GetLightsAsync()
    {
        return Task.FromResult(lights);
    }
    
    [KernelFunction("get_state")]
    [Description("Gets the state of a particular light")]
    [return: Description("The state of the light")]
    public async Task<LightModel?> GetStateAsync([Description("The ID of the light")] int id)
    {
        // Get the state of the light with the specified ID
        var light = lights.FirstOrDefault(light => light.Id == id);
        return light;
    }

    [KernelFunction("change_state")]
    [Description("Changes the state of the light")]
    [return: Description("The updated state of the light; will return null if the light does not exist")]
    public Task<LightModel?> ChangeStateAsync(int id, bool isOn)
    {
        var light = lights.FirstOrDefault(light => light.Id == id);

        if (light is null)
        {
            return Task.FromResult<LightModel?>(null);
        }

        // Update the light with the new state
        light.IsOn = isOn;

        return Task.FromResult<LightModel?>(light);
    }
}

public class LightModel
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("is_on")]
    public required bool? IsOn { get; set; }
}