using System.Collections.Generic;
using System.Threading.Tasks;
using Pulumi;
using Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class Program
{

    static Task<int> Main()
    {
        return Pulumi.Deployment.RunAsync(() =>
        {
            var config = new Pulumi.Config();
            var appName = "helloworld";

            var containers = new InputList<ContainerArgs>()
            {
                new ContainerArgs
                {
                    Name = appName,
                    Image = "gavdraper/weatherservice:again4",
                    Ports = {new ContainerPortArgs{ContainerPortValue = 80}}
                }
            };

            var deployment = KubeGen.DeploymentGenerator(appName, containers, 5);
            var frontend = KubeGen.ServiceGenerator(deployment);

            var ip = frontend.Status.Apply(status =>
                {
                    var ingress = status.LoadBalancer.Ingress[0];
                    return ingress.Ip ?? ingress.Hostname;
                });

            return new Dictionary<string, object?>
            {
                { "ip", ip },
            };
        });
    }
}