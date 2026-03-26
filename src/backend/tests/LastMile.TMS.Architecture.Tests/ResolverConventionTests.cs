using System.Reflection;
using FluentAssertions;
using LastMile.TMS.Application.Common.Interfaces;
using MediatR;

namespace LastMile.TMS.Architecture.Tests;

public class ResolverConventionTests
{
    private static readonly Assembly ApiAssembly = typeof(Api.Program).Assembly;

    /// <summary>
    /// GraphQL resolvers must not inject IAppDbContext or AppDbContext directly.
    /// </summary>
    [Fact]
    public void Resolvers_Must_Not_Inject_AppDbContext()
    {
        var resolverTypes = GetGraphQLResolverTypes();

        var violations = new List<string>();

        foreach (var type in resolverTypes)
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                foreach (var param in method.GetParameters())
                {
                    var paramType = param.ParameterType;
                    if (paramType == typeof(IAppDbContext) ||
                        paramType == typeof(Persistence.AppDbContext) ||
                        paramType.Name == "AppDbContext")
                    {
                        violations.Add($"{type.Name}.{method.Name} injects {paramType.Name}");
                    }
                }
            }
        }

        violations.Should().BeEmpty(
            because: "resolvers must not reach into AppDbContext directly");
    }

    /// <summary>
    /// Mutation resolvers must depend on ISender only.
    /// They must not inject read services directly.
    /// </summary>
    [Fact]
    public void MutationResolvers_Must_Use_ISender_Only()
    {
        var mutationTypes = GetGraphQLResolverTypes()
            .Where(t => t.Name.EndsWith("Mutation", StringComparison.Ordinal));

        var readServiceInterfaces = ApiAssembly.GetTypes()
            .Where(t => t.IsInterface && t.Name.EndsWith("ReadService", StringComparison.Ordinal))
            .ToHashSet();

        var violations = new List<string>();

        foreach (var type in mutationTypes)
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                foreach (var param in method.GetParameters())
                {
                    if (readServiceInterfaces.Contains(param.ParameterType))
                    {
                        violations.Add(
                            $"{type.Name}.{method.Name} injects read service {param.ParameterType.Name} " +
                            "- mutations must use ISender only");
                    }
                }
            }
        }

        violations.Should().BeEmpty(
            because: "mutation resolvers should depend on ISender only, not read services");
    }

    /// <summary>
    /// A single query resolver method must not inject both ISender and a read service.
    /// Each field should use one or the other, never both.
    /// </summary>
    [Fact]
    public void QueryResolvers_Must_Not_Mix_ISender_And_ReadService_In_Same_Method()
    {
        var queryTypes = GetGraphQLResolverTypes()
            .Where(t => t.Name.EndsWith("Query", StringComparison.Ordinal));

        var readServiceInterfaces = ApiAssembly.GetTypes()
            .Where(t => t.IsInterface && t.Name.EndsWith("ReadService", StringComparison.Ordinal))
            .ToHashSet();

        var violations = new List<string>();

        foreach (var type in queryTypes)
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                bool usesSender = false;
                bool usesReadService = false;

                foreach (var param in method.GetParameters())
                {
                    if (param.ParameterType == typeof(ISender))
                        usesSender = true;

                    if (readServiceInterfaces.Contains(param.ParameterType))
                        usesReadService = true;
                }

                if (usesSender && usesReadService)
                {
                    violations.Add(
                        $"{type.Name}.{method.Name} injects both ISender and a read service");
                }
            }
        }

        violations.Should().BeEmpty(
            because: "a query resolver field must use either ISender or a read service, never both");
    }

    private static List<Type> GetGraphQLResolverTypes()
    {
        return ApiAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                         && t.GetCustomAttributesData()
                             .Any(attr => attr.AttributeType.Name.Contains("ExtendObjectType")))
            .ToList();
    }
}
