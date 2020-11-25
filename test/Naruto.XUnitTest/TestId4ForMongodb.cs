
using Naruto.Id4.Entities;
using Naruto.MongoDB.Interface;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace Naruto.XUnitTest
{
    public class TestId4ForMongodb
    {
        private IServiceCollection services = new ServiceCollection();
        private readonly IMongoRepository<TestMongoContext> mongoRepository;
        public TestId4ForMongodb()
        {
            services.AddMongoServices().AddMongoContext<TestMongoContext>(a =>
            {
                a.ConnectionString = "mongodb://192.168.31.167:27021"; a.DataBase = "identityserver";
                // a.ConnectionString = "mongodb://192.168.1.6:27017"; a.DataBase = "identityserver";
            });
            mongoRepository = services.BuildServiceProvider().GetRequiredService<IMongoRepository<TestMongoContext>>();
        }
        /// <summary>
        /// 测试数据
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestData()
        {
            for (int i = 0; i < 100; i++)
            {
                //api资源
                Id4.Entities.ApiResource apiResource = new Id4.Entities.ApiResource
                {
                    Enabled = true,
                    Name = "api",
                    DisplayName = "api",
                    Description = "api",
                    Scopes = new List<ApiScope>()
                {
                    new Id4.Entities.ApiScope()
                    {
                        Name = "api",
                        DisplayName = "api",
                        Description = "api",
                        Required = true,
                        Emphasize = false,
                        ShowInDiscoveryDocument = true
                    }
                },
                    Secrets = null,
                    Properties = null,
                    UserClaims = null
                };


                await mongoRepository.Command<Id4.Entities.ApiResource>().AddAsync(apiResource);

                //客户端信息
                var client = new Id4.Entities.Client
                {
                    Enabled = true,
                    ClientId = "test" + i,
                    ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                    RequireClientSecret = true,
                    ClientName = "testname",
                    RequireConsent = true,
                    EnableLocalLogin = true,
                    AccessTokenLifetime = 7200,

                    //对象
                    ClientSecrets = new List<ClientSecret>()
                {
                    new ClientSecret
                    {
                        Expiration = DateTime.Now.AddYears(1),
                        Type = IdentityServerConstants.SecretTypes.SharedSecret,
                        Value = "123456".Sha256(),
                    }
                },
                    AllowedGrantTypes = new List<ClientGrantType>()
                {
                     new ClientGrantType{
                          GrantType=GrantType.ClientCredentials,
                     }
                },
                    AllowedScopes = new List<ClientScope>()
                {
                    new ClientScope{  Scope="api"}
                },
                    RedirectUris = null,
                    PostLogoutRedirectUris = null,
                    Claims = null,
                    AllowedCorsOrigins = null,
                    Properties = null,
                    IdentityProviderRestrictions = null,

                };
                await mongoRepository.Command<Id4.Entities.Client>().AddAsync(client);

                //新增设备
                await mongoRepository.Command<DeviceFlowCodes>().AddAsync(new DeviceFlowCodes
                {

                });

                //授权信息i
                await mongoRepository.Command<Id4.Entities.PersistedGrant>().AddAsync(new Id4.Entities.PersistedGrant
                {

                });
            }
            //认证资源
            //用与oidc的
            await mongoRepository.Command<Id4.Entities.IdentityResource>().AddAsync(new Id4.Entities.IdentityResource
            {
                Name = "openid",
                Enabled = true,
                Required = true,
                Created = DateTime.Now
            });
            await mongoRepository.Command<Id4.Entities.IdentityResource>().AddAsync(new Id4.Entities.IdentityResource
            {
                Name = "profile",
                Enabled = true,
                Required = true,
                Created = DateTime.Now
            });
        }
        /// <summary>
        /// 认证资源
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Identity()
        {
            //认证资源
            //用与oidc的
            await mongoRepository.Command<Id4.Entities.IdentityResource>().AddAsync(new Id4.Entities.IdentityResource
            {
                Name = "openid",
                Enabled = true,
                Required = true,
                Created = DateTime.Now
            });
            await mongoRepository.Command<Id4.Entities.IdentityResource>().AddAsync(new Id4.Entities.IdentityResource
            {
                Name = "profile",
                Enabled = true,
                Required = true,
                Created = DateTime.Now
            });
        }
    }
}
