﻿using System;
using System.Collections.Generic;
using Api.Modules.Templates.Models.Other;
using GeeksCoreLibrary.Modules.Templates.Enums;
using Newtonsoft.Json;

namespace Api.Modules.Templates.Models.Template
{
    /// <summary>
    /// A model for all settings of a template.
    /// </summary>
    public class TemplateSettingsModel
    {
        /// <summary>
        /// Gets or sets the ID of the template.
        /// </summary>
        public int TemplateId { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the parent.
        /// </summary>
        public int? ParentId { get; set; }
        
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the content/value of the template.
        /// This is HTML, (S)CSS, javascript etc, depending on the template type.
        /// </summary>
        public string EditorValue { get; set; }
        
        /// <summary>
        /// Gets or sets the minified value. Wiser minified (S)CSS and javascript when saving.
        /// </summary>
        [JsonIgnore]
        public string MinifiedValue { get; set; }
        
        /// <summary>
        /// Gets or sets the version number of the template.
        /// </summary>
        public int Version { get; set; }
        
        /// <summary>
        /// Gets or sets the date and time that the template was last changed.
        /// </summary>
        public DateTime ChangedOn { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the user that last changed the template.
        /// </summary>
        public string ChangedBy { get; set; }
        
        /// <summary>
        /// Gets or sets the template type, such as HTML, query, javascript etc.
        /// </summary>
        public TemplateTypes Type { get; set; }
        
        /// <summary>
        /// Gets or sets the ordering number of the template.
        /// The templates will be loaded in this order (ascending) in the tree view.
        /// When loading multiple templates on a page of a website, they will also be loaded in this order.
        /// </summary>
        public int Ordering { get; set; }
        
        /// <summary>
        /// Gets or sets the templates that are linked to this template. These are javascript or (S)CSS templates.
        /// </summary>
        public LinkedTemplatesModel LinkedTemplates { get; set; }
        
        /// <summary>
        /// Gets or sets which version of this template is deployed to which environment.
        /// </summary>
        public PublishedEnvironmentModel PublishedEnvironments { get; set; }

        #region HTML settings 
        /// <summary>
        /// Gets or sets the cache mode to use.
        /// </summary>
        public TemplateCachingModes UseCache { get; set; }
        
        /// <summary>
        /// Gets or sets the amount of minutes the template should stay in cache. Only used if <see cref="UseCache"/> is NOT set to <see cref="TemplateCachingModes.NoCaching"/>.
        /// </summary>
        public int CacheMinutes { get; set; }
        
        /// <summary>
        /// Gets or sets the location of where the cache should be saved.
        /// </summary>
        public TemplateCachingLocations CacheLocation { get; set; }
        
        /// <summary>
        /// Gets or sets a regex to decide on which pages this template should be cached.
        /// The template will be cached separately for each named group in this regex. 
        /// </summary>
        public string CacheRegex { get; set; }
        
        /// <summary>
        /// Gets or sets whether or not users should be logged in on the website to be able to open the page that contains this template.
        /// </summary>
        public bool LoginRequired { get; set; }
        
        /// <summary>
        /// Gets or sets the role(s) users need to have to be able to see/use this page/template.
        /// This only does something if <see cref="LoginRequired"/> is set to <see langword="true"/>.
        /// </summary>
        public List<int> LoginRoles { get; set; }
        
        /// <summary>
        /// Gets or sets the URL that users should be redirected to when they are not allowed to see/use this template,
        /// according to <see cref="LoginRequired"/> and <see cref="LoginRoles"/>.
        /// </summary>
        public string LoginRedirectUrl { get; set; }
        
        /// <summary>
        /// Gets or sets a query that can be executed before every time this template is loaded.
        /// This can be a SELECT query that returns data, then that data can be used as replacements in the content of the template.
        /// </summary>
        public string PreLoadQuery { get; set; }
        
        /// <summary>
        /// Gets or sets whether to return an HTTP 404 result if the pre load query returns no results, when someone opens this template.
        /// </summary>
        public bool ReturnNotFoundWhenPreLoadQueryHasNoData { get; set; }
        
        /// <summary>
        /// Gets or sets whether this template is the default header.
        /// If a template is the default header, it will be loaded at the beginning of every page.
        /// </summary>
        public bool IsDefaultHeader { get; set; }
        
        /// <summary>
        /// Gets or sets whether this template is the default footer.
        /// If a template is the default footer, it will be loaded at the end of every page.
        /// </summary>
        public bool IsDefaultFooter { get; set; }
        
        /// <summary>
        /// Gets or sets a regex that will be executed on the URL of each page to decide whether or not to load the header and footer on that page.
        /// </summary>
        public string DefaultHeaderFooterRegex { get; set; }
        
        /// <summary>
        /// Gets or sets whether this is a partial template. Partial templates will never load the header and footer templates
        /// and also not load any default CSS or javascript.
        /// </summary>
        public bool IsPartial { get; set; }
        
        /// <summary>
        /// Gets or sets the contents for widgets that should be loaded on this template. This can be HTML, javascript or CSS.
        /// </summary>
        public string WidgetContent { get; set; }
        
        /// <summary>
        /// Gets or sets the location in which the widget should be loaded.
        /// </summary>
        public PageWidgetLocations WidgetLocation { get; set; } = PageWidgetLocations.HeaderBottom;
        #endregion

        #region Css/Scss/Js settings.
        /// <summary>
        /// Gets or sets how and where the javascript or CSS should be loaded on the page.
        /// </summary>
        public ResourceInsertModes InsertMode { get; set; }
        
        /// <summary>
        /// Gets or sets whether this javascript or (S)CSS template should be loaded on every page. 
        /// </summary>
        public bool LoadAlways { get; set; }
        
        /// <summary>
        /// Gets or sets the URL regex to decide whether or not to load this javascript or (S)CSS template on a page.
        /// This is only used when <see cref="LoadAlways"/> is set to <see langword="true" />.
        /// </summary>
        public string UrlRegex { get; set; }
        
        /// <summary>
        /// Gets or sets any external javascript or CSS files that should be loaded.
        /// </summary>
        public List<string> ExternalFiles { get; set; } = new();
        
        /// <summary>
        /// Gets or sets whether this SCSS template is one that should be included before all other SCSS templates before compiling them.
        /// </summary>
        public bool IsScssIncludeTemplate { get; set; }
        
        /// <summary>
        /// Gets or sets whether this (S)CSS template should be loaded in all HTML editors (including ContentBuilder and ContentBox) in Wiser.
        /// </summary>
        public bool UseInWiserHtmlEditors { get; set; }
        #endregion

        #region Js only settings.
        public bool DisableMinifier { get; set; }
        #endregion
        
        #region Query settings
        /// <summary>
        /// Gets or sets whether to create a single object for the results of this query template, instead of an array.
        /// </summary>
        public bool GroupingCreateObjectInsteadOfArray { get; set; }
        
        /// <summary>
        /// Gets or sets the prefix that should be used for grouping results into a sub object.
        /// All columns that are meant to be added in a sub object, should start with this prefix.
        /// </summary>
        public string GroupingPrefix { get; set; }
        
        public string GroupingKey { get; set; }
        public string GroupingKeyColumnName { get; set; }
        public string GroupingValueColumnName { get; set; }
        #endregion

        #region Routine settings.
        public RoutineTypes RoutineType { get; set; }
        public string RoutineParameters { get; set; }
        public string RoutineReturnType { get; set; }
        #endregion

        #region Trigger settings.
        public string TriggerTableName { get; set; }
        public TriggerTimings TriggerTiming { get; set; }
        public TriggerEvents TriggerEvent { get; set; }
        #endregion
    }
}
