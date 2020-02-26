using System.Collections.Generic;
using System.Threading.Tasks;

using Pulumi;
using Pulumi.Kubernetes.Core.V1;
using Pulumi.Kubernetes.Apps.V1;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;
using Pulumi.Kubernetes.Types.Inputs.Apps.V1;
using Pulumi.Kubernetes.Types.Inputs.Meta.V1;

public static class KubeGen
{
    public static Service ServiceGenerator(Pulumi.Kubernetes.Apps.V1.Deployment deployment)
    {
        var appName = deployment.GetResourceName();
        var appLabels = new InputMap<string> { { "app", appName } };
        return new Service(appName, new ServiceArgs
        {
            Metadata = new ObjectMetaArgs
            {
                Labels = deployment.Spec.Apply(spec =>
                    spec.Template.Metadata.Labels
                    ),
            },
            Spec = new ServiceSpecArgs
            {
                Type = "LoadBalancer",
                Selector = appLabels,
                Ports = new ServicePortArgs
                {
                    Port = 80,
                    TargetPort = 80,
                    Protocol = "TCP",
                },
            }
        });
    }

    public static Pulumi.Kubernetes.Apps.V1.Deployment DeploymentGenerator(string appName, InputList<ContainerArgs> containers, int replicas = 1)
    {
        var appLabels = new InputMap<string> { { "app", appName } };
        var deployment = new Pulumi.Kubernetes.Apps.V1.Deployment(appName, new DeploymentArgs
        {
            Spec = new DeploymentSpecArgs
            {
                Selector = new LabelSelectorArgs
                {
                    MatchLabels = appLabels,
                },
                Replicas = replicas,
                Template = new PodTemplateSpecArgs
                {
                    Metadata = new ObjectMetaArgs
                    {
                        Labels = appLabels,
                    },
                    Spec = new PodSpecArgs
                    {
                        Containers = containers
                    },
                },
            },
        });
        return deployment;
    }
}