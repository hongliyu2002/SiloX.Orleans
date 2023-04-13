using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;

namespace SiloX.AspNetCore.Components.DependencyInjection;

/// <summary>
///     A <see cref="IComponentActivator" /> that uses the <see cref="IServiceProvider" /> to create components.
/// </summary>
[PublicAPI]
public class ServiceProviderComponentActivator : IComponentActivator
{

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceProviderComponentActivator" /> class.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider" /> used to create components.</param>
    public ServiceProviderComponentActivator(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    /// <summary>
    ///     Gets the <see cref="IServiceProvider" /> used to create components.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    ///     Creates a new instance of the specified component type.
    /// </summary>
    /// <param name="componentType">The type of the component to create.</param>
    /// <returns>The created component.</returns>
    /// <exception cref="ArgumentException">Thrown if the specified type does not implement <see cref="IComponent" />.</exception>
    public IComponent CreateInstance(Type componentType)
    {
        var instance = ServiceProvider.GetService(componentType) ?? Activator.CreateInstance(componentType);
        if (instance is not IComponent component)
        {
            throw new ArgumentException($"The type {componentType.FullName} does not implement {nameof(IComponent)}.", nameof(componentType));
        }
        return component;
    }
}