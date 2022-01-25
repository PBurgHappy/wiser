﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Core.Helpers;
using Api.Modules.Templates.Helpers;
using Api.Modules.Templates.Interfaces;
using Api.Modules.Templates.Interfaces.DataLayer;
using GeeksCoreLibrary.Core.Cms;
using GeeksCoreLibrary.Core.Cms.Attributes;
using GeeksCoreLibrary.Core.DependencyInjection.Interfaces;
using static GeeksCoreLibrary.Core.Cms.Attributes.CmsAttributes;

namespace Api.Modules.Templates.Services
{
    /// <inheritdoc cref="IDynamicContentService" />
    public class DynamicContentService : IDynamicContentService, IScopedService
    {
        private readonly IDynamicContentDataService dataService;

        /// <summary>
        /// Creates a new instance of <see cref="DynamicContentService"/>.
        /// </summary>
        public DynamicContentService(IDynamicContentDataService dataService)
        {
            this.dataService = dataService;
        }

        /// <inheritdoc />
        public Dictionary<TypeInfo, CmsObjectAttribute> GetComponents()
        {
            var componentType = typeof(CmsComponent<CmsSettings, Enum>);
            var assembly = componentType.Assembly;

            var typeInfoList = assembly.DefinedTypes.Where(
                type => type.BaseType != null
                && type.BaseType.IsGenericType
                && componentType.IsGenericType
                && type.BaseType.GetGenericTypeDefinition() == componentType.GetGenericTypeDefinition()
            ).OrderBy(type => type.Name).ToList();

            var resultDictionary = new Dictionary<TypeInfo, CmsObjectAttribute>();

            foreach (var typeInfo in typeInfoList)
            {
                resultDictionary.Add(typeInfo, typeInfo.GetCustomAttribute<CmsObjectAttribute>());
            }
            return resultDictionary;
        }

        /// <inheritdoc />
        public Dictionary<object, string> GetComponentModes(Type component)
        {
            var info = (component.BaseType).GetTypeInfo();
            var enumtype = info.GetGenericArguments()[1];
            var enumFields = enumtype.GetFields();

            var returnDict = new Dictionary<object, string>();

            foreach (var enumField in enumFields)
            {
                if (enumField.Name.Equals("value__")) continue;
                returnDict.Add(enumField.GetRawConstantValue(), enumField.Name);
            }

            return returnDict;
        }

        /// <inheritdoc />
        public List<PropertyInfo> GetPropertiesOfType(Type CmsSettingsType)
        {
            var resultlist = new List<PropertyInfo>();
            var allProperties = CmsSettingsType.GetProperties();

            foreach (var property in allProperties)
            {
                resultlist.Add(property);
            }

            return resultlist;
        }

        /// <inheritdoc />
        public KeyValuePair<Type, Dictionary<CmsTabName, Dictionary<CmsGroupName, Dictionary<PropertyInfo, CmsPropertyAttribute>>>> GetAllPropertyAttributes(Type component)
        {
            var resultList = new Dictionary<CmsTabName, Dictionary<CmsGroupName, Dictionary<PropertyInfo, CmsPropertyAttribute>>>();
            var CmsSettingsType = new ReflectionHelper().GetCmsSettingsType(component);
            var propInfos = CmsSettingsType.GetProperties().Where(propInfo => propInfo.GetCustomAttribute<CmsPropertyAttribute>() != null).OrderBy(propInfo => propInfo.GetCustomAttribute<CmsPropertyAttribute>().DisplayOrder);

            foreach (var propInfo in propInfos)
            {
                var cmsPropertyAttribute = propInfo.GetCustomAttribute<CmsPropertyAttribute>();

                if (cmsPropertyAttribute == null)
                {
                    continue;
                }

                if (resultList.ContainsKey(cmsPropertyAttribute.TabName))
                {
                    if (resultList[cmsPropertyAttribute.TabName].ContainsKey(cmsPropertyAttribute.GroupName))
                    {
                        resultList[cmsPropertyAttribute.TabName][cmsPropertyAttribute.GroupName].Add(propInfo, cmsPropertyAttribute);
                    }
                    else
                    {
                        var cmsGroup = CreateCmsGroupFromPropertyAttribute(propInfo, cmsPropertyAttribute);
                        foreach (var kvp in cmsGroup)
                        {
                            resultList[cmsPropertyAttribute.TabName].Add(kvp.Key, kvp.Value);
                        }
                    }
                }
                else
                {
                    resultList.Add(cmsPropertyAttribute.TabName, CreateCmsGroupFromPropertyAttribute(propInfo, cmsPropertyAttribute));
                }
            }
            return new KeyValuePair<Type, Dictionary<CmsTabName, Dictionary<CmsGroupName, Dictionary<PropertyInfo, CmsPropertyAttribute>>>>(component, resultList); ;
        }

        /// <inheritdoc />
        public async Task<Dictionary<PropertyInfo, object>> GetCmsSettingsModel(Type component, int templateId)
        {
            var CmsSettingsType = new ReflectionHelper().GetCmsSettingsType(component);
            var properties = GetPropertiesOfType(CmsSettingsType);
            var data = (await dataService.GetTemplateData(templateId)).Value;

            var accountSettings = new Dictionary<PropertyInfo, object>();

            if (data == null)
            {
                return null;
            }

            foreach (var property in properties)
            {
                object value = null;

                //Find matching data
                foreach (var setting in data)
                {
                    if (property.Name.Equals(setting.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        value = setting.Value;
                    }
                }

                accountSettings.Add(property, value);

                if (value == null || value.GetType() == typeof(string) && string.IsNullOrEmpty((string)value))
                {
                }
            }

            return accountSettings;
        }

        /// <inheritdoc />
        public async Task<int> SaveNewSettings(ClaimsIdentity identity, int contentId, string component, int componentMode, string title, Dictionary<string, object> settings)
        {
            var helper = new ReflectionHelper();
            var modes = GetComponentModes(helper.GetComponentTypeByName(component));
            modes.TryGetValue(componentMode, out var componentModeName);
            return await dataService.SaveSettingsString(contentId, component, componentModeName, title, settings, IdentityHelpers.GetUserName(identity));
        }

        /// <inheritdoc />
        public async Task<List<string>> GetComponentAndModeForContentId(int contentId)
        {
            var rawComponentAndMode = await dataService.GetComponentAndModeFromContentId(contentId);

            return rawComponentAndMode;
        }

        /// <summary>
        /// Creates a new dictionary for properties with the same group.
        /// </summary>
        /// <param name="propName">The name of the property which will be used as the key for the property within the group.</param>
        /// <param name="cmsPropertyAttribute">The CmsAttribute belonging to the attribut within the property group.</param>
        /// <returns>
        /// Dictionary of a cms group. The key is the cmsgroup name and the value is a propertyattribute dictionary.
        /// </returns>
        private Dictionary<CmsGroupName, Dictionary<PropertyInfo, CmsPropertyAttribute>> CreateCmsGroupFromPropertyAttribute(PropertyInfo propName, CmsPropertyAttribute cmsPropertyAttribute)
        {
            var cmsGroup = new Dictionary<CmsGroupName, Dictionary<PropertyInfo, CmsPropertyAttribute>>();
            var propList = new Dictionary<PropertyInfo, CmsPropertyAttribute>();

            propList.Add(propName, cmsPropertyAttribute);
            cmsGroup.Add(cmsPropertyAttribute.GroupName, propList);

            return cmsGroup;
        }
    }
}
