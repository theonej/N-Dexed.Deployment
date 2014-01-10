using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.ElasticBeanstalk;
using Amazon.ElasticBeanstalk.Model;
using Amazon.Route53;
using Amazon.Route53.Model;
using Amazon.Runtime;
using Amazon.SimpleNotificationService.Model;
using CuttingEdge.Conditions;
using N_Dexed.Deployment.Common.Domain.Management;
using N_Dexed.Deployment.Common.Domain.Repositories;
using N_Dexed.Deployment.Common.Domain.Systems;
using N_Dexed.Deployment.Common.Resources;

namespace N_Dexed.Deployment.AWS.Management
{
    public class ElasticBeanstalkApplicationInterface : IApplicationInterface
    {
        private const string US_EAST_ENDPOINT_URL = "https://elasticbeanstalk.us-east-1.amazonaws.com";

        private readonly IRepository<SystemInfo> m_SystemRepository;

        public ElasticBeanstalkApplicationInterface(IRepository<SystemInfo> systemRepository)
        {
            Condition.Requires(systemRepository).IsNotNull();

            m_SystemRepository = systemRepository;
        }

        public List<ApplicationInfo> GetApplications(Guid systemId)
        {
            List<ApplicationInfo> applications = new List<ApplicationInfo>();

            SystemInfo system = GetSystem(systemId);
            AmazonElasticBeanstalkClient client = InitializeClient(system);
            using (client)
            {
                DescribeApplicationsResponse response = client.DescribeApplications();
                foreach (ApplicationDescription application in response.Applications)
                {
                    ApplicationInfo applicationInfo = new ApplicationInfo
                    {
                        ApplicationName = application.ApplicationName,
                        Description = application.Description,
                        CreatedDateTime = application.DateCreated,
                        SystemId = systemId,
                        ApplicationVersions = application.Versions,
                        Id = Guid.NewGuid(),
                        ApplicationType = ApplicationTypes.ElasticBeanstalk
                    };

                    applications.Add(applicationInfo);
                }
            }

            return applications;
        }

        public List<EnvironmentInfo> GetEnvironments(Guid systemId, string applicationName)
        {
            List<EnvironmentInfo> environments = new List<EnvironmentInfo>();

             ApplicationInfo applicationInfo = GetApplication(systemId, applicationName);
             SystemInfo system = GetSystem(systemId);

            AmazonElasticBeanstalkClient client = InitializeClient(system);
            using (client)
            {
                DescribeEnvironmentsRequest request = new DescribeEnvironmentsRequest();
                request.ApplicationName = applicationName;
                
                DescribeEnvironmentsResponse response = client.DescribeEnvironments(request);
                foreach (EnvironmentDescription description in response.Environments)
                {
                    EnvironmentInfo environment = new EnvironmentInfo();
                    environment.Description = description.Description;
                    environment.DnsName = description.CNAME;
                    environment.EnvironmentName = description.EnvironmentName;
                    environment.EndpointURL = description.EndpointURL;
                    environment.Health = description.Health;
                    environment.Status = description.Status;
                    environment.Version = description.VersionLabel;
                    environment.DNSPointerRecords = GetEnvironmentRouteDNSName(applicationName, system, environment);

                    environments.Add(environment);
                }
            }

            return environments;
        }

        public ApplicationInfo GetApplication(Guid systemId, string applicationName)
        {
            List<ApplicationInfo> applications = GetApplications(systemId);

            ApplicationInfo returnValue = (
                                                from
                                                    records
                                                in
                                                    applications
                                                where
                                                    records.ApplicationName == applicationName
                                                select
                                                    records
                                          ).FirstOrDefault();

            return returnValue;
        }

        #region Private Methods

        private AmazonElasticBeanstalkClient InitializeClient(SystemInfo system)
        {
            AmazonElasticBeanstalkConfig config = new AmazonElasticBeanstalkConfig();
            config.ServiceURL = US_EAST_ENDPOINT_URL;

            AmazonElasticBeanstalkClient client = new AmazonElasticBeanstalkClient(system.Credentials.AccessKey, system.Credentials.SecretKey, config);
            
            return client;
        }


        private SystemInfo GetSystem(Guid systemId)
        {
            SystemInfo system = new SystemInfo();
            system.Id = systemId;

            system = m_SystemRepository.Search(system).FirstOrDefault();

            if (system == null)
            {
                string errorMessage = string.Format(ErrorMessages.SystemNotFound, systemId);
                throw new DataMisalignedException(errorMessage);
            }

            return system;
        }

        private List<string> GetEnvironmentRouteDNSName(string applicationName, SystemInfo system, EnvironmentInfo environment)
        {
            List<string> returnValue = new List<string>();

            AmazonRoute53Config config = new AmazonRoute53Config();
            config.RegionEndpoint = RegionEndpoint.USEast1;

            AmazonRoute53Client client = new AmazonRoute53Client(system.Credentials.AccessKey, system.Credentials.SecretKey, config);
            using (client)
            {
                ListHostedZonesResponse response = client.ListHostedZones();
                foreach (HostedZone zone in response.HostedZones)
                {
                    ListResourceRecordSetsRequest recordSetRequest= new ListResourceRecordSetsRequest();
                    recordSetRequest.HostedZoneId = zone.Id;

                    ListResourceRecordSetsResponse recordSetsResponse = client.ListResourceRecordSets(recordSetRequest);
                    foreach (ResourceRecordSet recordSet in recordSetsResponse.ResourceRecordSets)
                    {
                        bool match = (
                                        from
                                            records
                                        in
                                            recordSet.ResourceRecords
                                        where
                                            records.Value == environment.DnsName
                                            ||
                                            records.Value == environment.EndpointURL
                                        select
                                            records
                                        ).Any();

                        if (match)
                        {
                            returnValue.Add(recordSet.Name);
                        }
                    }
                }
            }

            return returnValue;
        }

        #endregion
    }
}
