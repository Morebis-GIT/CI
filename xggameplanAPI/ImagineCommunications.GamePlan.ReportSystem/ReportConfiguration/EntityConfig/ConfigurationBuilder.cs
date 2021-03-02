using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.EntityConfig
{
    public abstract class ConfigurationBuilder<TEntity, TMemberOptionsBuilder, TBuilder, TReportConfig, TMemberOptions>
        : IConfigurationBuilder<TEntity, TMemberOptionsBuilder, TBuilder, TReportConfig>
        where TBuilder : ConfigurationBuilder<TEntity, TMemberOptionsBuilder, TBuilder, TReportConfig, TMemberOptions>
        where TEntity : class
        where TMemberOptions: class, IMemberOptions
        where TMemberOptionsBuilder : IMemberOptionsBuilder<TMemberOptionsBuilder, TMemberOptions>
        where TReportConfig : IReportConfigurations
    {
        protected virtual IConfigurationOptions Options { get; set; }

        protected Dictionary<MemberInfo, int> MappedMemberInfos { get; } = new Dictionary<MemberInfo, int>();
        protected int MappedMemberInfosCount { get; set; } = 0;

        protected bool IgnoredNotDescribed;
        protected bool SetOrdinalMembersAsDescribed;

        protected virtual IConfigurationDictionary<TMemberOptionsBuilder, TMemberOptions> ConfigurationDictionary { get; set; }

        public ConfigurationBuilder(IConfigurationOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            ConfigurationDictionary = new ConfigurationDictionary<TMemberOptionsBuilder, TMemberOptions>();
            InitAllMembers();
        }

        public TBuilder UseHumanizeHeader(bool useHeaderHumanizer = true)
        {
            Options.UseHeaderHumanizer = useHeaderHumanizer;
            return (TBuilder)this;
        }

        public TBuilder OrderMembersAsDescribed(bool orderMembersAsDescribed = true)
        {
            SetOrdinalMembersAsDescribed = orderMembersAsDescribed;
            return (TBuilder)this;
        }

        public TBuilder IgnoreNotDescribed()
        {
            IgnoredNotDescribed = true;
            return (TBuilder)this;
        }

        public virtual TBuilder HideHeader()
        {
            return HideHeader(true);
        }

        public virtual TBuilder HideHeader(bool isHideHeader)
        {
            Options.IsHideHeader = isHideHeader;
            return (TBuilder)this;
        }

        public virtual TBuilder ForMember(Expression<Func<TEntity, object>> memberExpression, Action<TMemberOptionsBuilder> memberOptions)
        {
            var member = TypeAccessor.TypeAccessor.GetMember(memberExpression);

            return ForMember(memberOptions, member);
        }

        public virtual TBuilder ForMember(string memberName, Action<TMemberOptionsBuilder> memberOptions)
        {
            var member = TypeAccessor.TypeAccessor.GetMember(typeof(TEntity), memberName);
            return ForMember(memberOptions, member);
        }

        private TBuilder ForMember(Action<TMemberOptionsBuilder> memberOptions, MemberInfo member)
        {
            if (!MappedMemberInfos.ContainsKey(member))
            {
                MappedMemberInfosCount++;
                MappedMemberInfos.Add(member, MappedMemberInfosCount);
            }

            var expression = GetAndStoreExpression(member);
            memberOptions(expression);

            return (TBuilder)this;
        }

        public abstract TReportConfig BuildConfiguration();

        protected TReportConfig BuildConfiguration<TConfOptions>()
            where TConfOptions : IConfigurationOptions
        {
            SetIgnoredToNotMapped();
            SetOrderMembersAsDescribed();

            var memberConfigurations = new MemberConfigurationDictionary<TMemberOptions>();
            foreach (var configurationExpression in ConfigurationDictionary)
            {
                memberConfigurations.Add(configurationExpression.Key, configurationExpression.Value.Build());
            }

            object[] args = { (TConfOptions)Options, memberConfigurations };
            return (TReportConfig) Activator.CreateInstance(typeof(TReportConfig), args);
        }

        protected virtual void SetIgnoredToNotMapped()
        {
            if (!IgnoredNotDescribed)
            {
                return;
            }

            foreach (var configurationExpression in ConfigurationDictionary)
            {
                if(!MappedMemberInfos.ContainsKey(configurationExpression.Key))
                {
                    configurationExpression.Value.Ignore();
                }
            }
        }

        protected virtual void SetOrderMembersAsDescribed()
        {
            if (!SetOrdinalMembersAsDescribed)
            {
                return;
            }

            foreach (var configurationExpression in ConfigurationDictionary)
            {
                if (MappedMemberInfos.ContainsKey(configurationExpression.Key))
                {
                    configurationExpression.Value.Order(MappedMemberInfos[configurationExpression.Key]);
                }
            }
        }

        protected virtual TMemberOptionsBuilder GetAndStoreExpression(MemberInfo member)
        {
            if (!ConfigurationDictionary.ContainsKey(member))
            {
                object[] args = { member.Name };
                ConfigurationDictionary.Add(member, (TMemberOptionsBuilder)Activator.CreateInstance(typeof(TMemberOptionsBuilder), args));
            }

            return ConfigurationDictionary[member];
        }

        protected virtual void InitAllMembers()
        {
            var members = TypeAccessor.TypeAccessor.GetAllMembers(typeof(TEntity));
            var order = 1;
            foreach (var member in members)
            {
                var expression = GetAndStoreExpression(member);
                expression
                    .Order(order);

                order++;
            }
        }
    }
}
