﻿// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
namespace Rock.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    /// <summary>
    ///
    /// </summary>
    public partial class MissingPluginMigrations : Rock.Migrations.RockMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            HomepageMatrix();
            DuplicateDataIntegrityReports();
            MigrationRollupsForV8_2();
            UpdateWordcloudShortcode();
            EnableNCOA();
            CheckinRegistration();

        }
        
        /// <summary>
        /// GJ: Default Values IF move adult children is not enabled.
        /// This migration was originally in 052_MigrationRollupsForV8_1
        /// and was not included in an actual migration.
        /// </summary>
        private void HomepageMatrix()
        {
            Sql( @"UPDATE [dbo].[AttributeMatrixTemplate] SET [FormattedLava] = N'{% if AttributeMatrixItems != empty %}
<div class=""row"">
{% for attributeMatrixItem in AttributeMatrixItems %}
    <div class=""homepage-article col-md-4"">
      <div class=""photo"" style=""background-image: url(''{{ '''' | ResolveRockUrl }}/GetImage.ashx?Guid={{ attributeMatrixItem | Attribute:''ArticleImage'',''RawValue'' }}'')"" alt=""{{ attributeMatrixItem | Attribute:''ArticleTitle'' }}""></div>
      <div class=""body"">
        <h5 class=""title"">{{ attributeMatrixItem | Attribute:''ArticleTitle'' }}</h5>
        <div class=""body"">{{ attributeMatrixItem | Attribute:''ArticleContent'' }}</div>
        {% assign articleLink = attributeMatrixItem | Attribute:''ArticleLink'',''RawValue'' -%}
        {% if articleLink != empty -%}
            <a class=""btn btn-xs btn-link"" href=""{{ articleLink }}"">More Info</a>
        {% endif -%}
     </div>
    </div>
    {% cycle '''', '''', ''</div><div class=""row"">'' %}
{% endfor %}
</div>
{% endif %}'
WHERE [Guid] = '1d24694e-445c-4852-b5bc-64cdea6f7175'
AND [FormattedLava] = N'{% if AttributeMatrixItems != empty %}
<div class=""row"">
{% for attributeMatrixItem in AttributeMatrixItems %}
    <div class=""homepage - article col - md - 4"">
      < div class=""photo"" style=""background-image: url(''./GetImage.ashx?Guid={{ attributeMatrixItem | Attribute:''ArticleImage'',''RawValue'' }}"" alt=""{{ attributeMatrixItem | Attribute:''ArticleTitle'' }}'');""></div>
      <div class=""body"">
        <h5 class=""title"">{{ attributeMatrixItem | Attribute:''ArticleTitle'' }
}</h5>
        <div class=""body"">{{ attributeMatrixItem | Attribute:''ArticleContent'' }}</div>
        {% assign articleLink = attributeMatrixItem | Attribute:''ArticleLink'',''RawValue'' -%}
        {% if articleLink != empty -%}
            <a class=""btn btn-xs btn-link"" href=""{{ articleLink }}"">More Info</a>
        {% endif -%}
     </div>
    </div>
    {% cycle '''', '''', ''</div><div class=""row"">'' %}
{% endfor %}
</div>
{% endif %}'" );
        }

        /// <summary>
        /// Migration from 053_DuplicateDataIntegrityReports
        /// Made minor changes to formatting and to ensure the migration is multi-run safe.
        /// </summary>
        private void DuplicateDataIntegrityReports()
        {
            RockMigrationHelper.UpdateEntityType( "Rock.Reporting.DataFilter.Person.HasDuplicatePhoneFilter", "Has Duplicate Phone Filter", "Rock.Reporting.DataFilter.Person.HasDuplicatePhoneFilter, Rock, Version=1.8.1.0, Culture=neutral, PublicKeyToken=null", false, true, "32ea43cc-8744-4180-9bc7-9915f467e54b" );
            RockMigrationHelper.UpdateEntityType( "Rock.Reporting.DataFilter.Person.HasDuplicateEmailFilter", "Has Duplicate Email Filter", "Rock.Reporting.DataFilter.Person.HasDuplicateEmailFilter, Rock, Version=1.8.1.0, Culture=neutral, PublicKeyToken=null", false, true, "2771a757-dd80-420c-a128-a9fb24f40da9" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Reporting.DataFilter.Person.HasDuplicateEmailFilter", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "", "", "Order", "The order that this service should be used (priority)", 0, "", "805303CC-35B7-4C49-AD2D-C93D59F53494" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Reporting.DataFilter.Person.HasDuplicateEmailFilter", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "", "", "Active", "Should Service be used?", 0, "False", "416114CC-6F31-4ACC-85C9-8641B5239AB0" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Reporting.DataFilter.Person.HasDuplicatePhoneFilter", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "", "", "Order", "The order that this service should be used (priority)", 0, "", "57D6A71E-EC69-45D4-819E-36506C7DD3FA" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Reporting.DataFilter.Person.HasDuplicatePhoneFilter", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "", "", "Active", "Should Service be used?", 0, "False", "25CC2F66-1CFF-4C0E-9AAB-F17F3C678993" );

            // Create [GroupAll] DataViewFilter for DataView: People with Duplicate Phone Numbers
            Sql( @"
                IF NOT EXISTS (SELECT * FROM [DataViewFilter] WHERE [Guid] = 'ECD5554E-9F69-46E2-861C-4238A0B8428A')
                BEGIN
                    INSERT INTO [DataViewFilter] ([ExpressionType], [ParentId], [EntityTypeId], [Selection], [Guid]) 
                    VALUES (1, NULL, NULL, '', 'ECD5554E-9F69-46E2-861C-4238A0B8428A')
                END" );

            // Create Rock.Reporting.DataFilter.Person.HasDuplicatePhoneFilter DataViewFilter for DataView: People with Duplicate Phone Numbers
            Sql( @"
                IF NOT EXISTS (SELECT * FROM [DataViewFilter] WHERE [Guid] = '4EB44900-7381-4BCF-B527-1B4571134828')
                BEGIN
                    DECLARE
                        @ParentDataViewFilterId int = (SELECT [Id] FROM [DataViewFilter] WHERE [Guid] = 'ECD5554E-9F69-46E2-861C-4238A0B8428A'),
                        @DataViewFilterEntityTypeId int = (SELECT [Id] FROM [EntityType] WHERE [Guid] = '32EA43CC-8744-4180-9BC7-9915F467E54B')

                    INSERT INTO [DataViewFilter] ([ExpressionType], [ParentId], [EntityTypeId], [Selection], [Guid]) 
                    VALUES (0, @ParentDataViewFilterId, @DataViewFilterEntityTypeId, '', '4EB44900-7381-4BCF-B527-1B4571134828')
                END" );

            // Create DataView: People with Duplicate Phone Numbers
            Sql( @"
                IF NOT EXISTS (SELECT * FROM [DataView] WHERE [Guid] = '93AFFF40-CA13-4F19-8BCA-CC5CF2896384')
                BEGIN
                    DECLARE
                        @categoryId int = (SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = 'BDD2C36F-7575-48A8-8B70-3A566E3811ED'),
                        @entityTypeId  int = (SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = '72657ED8-D16E-492E-AC12-144C5E7567E7'),
                        @dataViewFilterId  int = (SELECT TOP 1 [Id] FROM [DataViewFilter] WHERE [Guid] = 'ECD5554E-9F69-46E2-861C-4238A0B8428A')

                    INSERT INTO [DataView] ([IsSystem], [Name], [Description], [CategoryId], [EntityTypeId], [DataViewFilterId], [TransformEntityTypeId], [Guid])
                    VALUES(0, 'People with Duplicate Phone Numbers', 'People who have duplicate phone numbers.', @categoryId, @entityTypeId, @dataViewFilterId, NULL, '93AFFF40-CA13-4F19-8BCA-CC5CF2896384')
                END" );

            // Create [GroupAll] DataViewFilter for DataView: People with Duplicate Emails
            Sql( @"
                IF NOT EXISTS (SELECT * FROM [DataViewFilter] WHERE [Guid] = '0BC16586-B9D1-49B4-9E4E-2AF69A0C63A5')
                BEGIN
                    INSERT INTO [DataViewFilter] ([ExpressionType], [ParentId], [EntityTypeId], [Selection], [Guid]) 
                    VALUES (1, NULL, NULL, '', '0BC16586-B9D1-49B4-9E4E-2AF69A0C63A5')
                END" );

            // Create Rock.Reporting.DataFilter.Person.HasDuplicateEmailFilter DataViewFilter for DataView: People with Duplicate Emails
            Sql( @"
                IF NOT EXISTS (SELECT * FROM [DataViewFilter] WHERE [Guid] = 'CE323E30-5C26-4AE6-AF37-C39B9A05192B')
                BEGIN
                    DECLARE
                        @ParentDataViewFilterId int = (SELECT [Id] FROM [DataViewFilter] WHERE [Guid] = '0BC16586-B9D1-49B4-9E4E-2AF69A0C63A5'),
                        @DataViewFilterEntityTypeId int = (SELECT [Id] FROM [EntityType] WHERE [Guid] = '2771A757-DD80-420C-A128-A9FB24F40DA9')

                    INSERT INTO [DataViewFilter] ([ExpressionType], [ParentId], [EntityTypeId], [Selection], [Guid]) 
                    VALUES (0, @ParentDataViewFilterId, @DataViewFilterEntityTypeId, '', 'CE323E30-5C26-4AE6-AF37-C39B9A05192B')
                END" );

            // Create DataView: People with Duplicate Emails
            Sql( @"
                IF NOT EXISTS (SELECT * FROM [DataView] WHERE [Guid] = 'E62BBEC8-2B7B-41AF-9598-28B2BF21944D')
                BEGIN
                DECLARE
                    @categoryId int = (SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = 'BDD2C36F-7575-48A8-8B70-3A566E3811ED'),
                    @entityTypeId  int = (SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = '72657ED8-D16E-492E-AC12-144C5E7567E7'),
                    @dataViewFilterId  int = (SELECT TOP 1 [Id] FROM [DataViewFilter] WHERE [Guid] = '0BC16586-B9D1-49B4-9E4E-2AF69A0C63A5')

                INSERT INTO [DataView] ([IsSystem], [Name], [Description], [CategoryId], [EntityTypeId], [DataViewFilterId], [TransformEntityTypeId], [Guid])
                VALUES(0, 'People with Duplicate Emails', 'People who have duplicate email addresses.', @categoryId, @entityTypeId, @dataViewFilterId, NULL,'E62BBEC8-2B7B-41AF-9598-28B2BF21944D')
                END" );

            RockMigrationHelper.AddReport( "d738d12d-bc3b-47b0-8a90-f7924d137595", "93afff40-ca13-4f19-8bca-cc5cf2896384", "72657ed8-d16e-492e-ac12-144c5e7567e7", "Individuals with Duplicate Phone Numbers", "People who have duplicate phone numbers.", "cccbf8d5-3968-42d1-87e9-ca909a9b5d00" );
            RockMigrationHelper.AddReport( "d738d12d-bc3b-47b0-8a90-f7924d137595", "e62bbec8-2b7b-41af-9598-28b2bf21944d", "72657ed8-d16e-492e-ac12-144c5e7567e7", "Individuals with Duplicate Emails", "People who have duplicate email addresses.", "69d012db-bebd-4a83-bfb6-880655e1b571" );
            RockMigrationHelper.AddReportField( "cccbf8d5-3968-42d1-87e9-ca909a9b5d00", Model.ReportFieldType.DataSelectComponent, true, "6301f6b4-b2ef-469a-8ec2-7d5f06b55c60", "{\"ShowAsLink\":\"True\",\"DisplayOrder\":\"0\"}", 0, "Name", "04532a6c-0bc7-4eb6-ae5b-d636e0707fff" );
            RockMigrationHelper.AddReportField( "cccbf8d5-3968-42d1-87e9-ca909a9b5d00", Model.ReportFieldType.DataSelectComponent, true, "5cbfbc11-826a-4b83-845e-247dd6268fff", "407e7e45-7b2e-4fcd-9605-ecb1339f2453|False", 1, "Mobile Phone", "2a0961fd-f658-4989-b23d-9a6d2762ff7c" );
            RockMigrationHelper.AddReportField( "cccbf8d5-3968-42d1-87e9-ca909a9b5d00", Model.ReportFieldType.DataSelectComponent, true, "5cbfbc11-826a-4b83-845e-247dd6268fff", "aa8732fb-2cea-4c76-8d6d-6aaa2c6a4303|False", 2, "Home Phone", "a90872df-614a-4a82-b6f6-d83838a9b184" );
            RockMigrationHelper.AddReportField( "cccbf8d5-3968-42d1-87e9-ca909a9b5d00", Model.ReportFieldType.DataSelectComponent, true, "5cbfbc11-826a-4b83-845e-247dd6268fff", "2cc66d5a-f61c-4b74-9af9-590a9847c13c|False", 3, "Work Phone", "92f93f8d-8b5c-4caf-9338-2d117fe273c7" );
            RockMigrationHelper.AddReportField( "69d012db-bebd-4a83-bfb6-880655e1b571", Model.ReportFieldType.DataSelectComponent, true, "6301f6b4-b2ef-469a-8ec2-7d5f06b55c60", "{\"ShowAsLink\":\"True\",\"DisplayOrder\":\"0\"}", 0, "Name", "60efff7b-10e7-4784-9e71-d9246b785cf4" );
            RockMigrationHelper.AddReportField( "69d012db-bebd-4a83-bfb6-880655e1b571", Model.ReportFieldType.Property, true, new Guid().ToString(), "Email", 1, "Email", "4fe499da-8c2a-463a-ad2f-87045ccdfaa4" );
        }

        /// <summary>
        /// Migration from 054_MigrationRollupsForV8_2
        /// </summary>
        private void MigrationRollupsForV8_2()
        {
            // SK: Update group view lava template to display schedule when exist( ISSUE #3188)
            // Update the default template
            Sql( @"		  
            	UPDATE [GroupType]
                SET [GroupViewLavaTemplate] = REPLACE([GroupViewLavaTemplate],'{{ Group.Schedule.ToString()','{{ Group.Schedule.FriendlyScheduleText')
                WHERE [GroupViewLavaTemplate] Like '%{{ Group.Schedule.ToString()%'" );

            // Update the system setting 
            Sql( @"
                UPDATE [Attribute]
                SET [DefaultValue] = REPLACE([DefaultValue],'{{ Group.Schedule.ToString()','{{ Group.Schedule.FriendlyScheduleText')
                WHERE [Key]='core_templates_GroupViewTemplate'" );

            // SK: Added Other to Currency Type Defined Type
            RockMigrationHelper.UpdateDefinedValue( Rock.SystemGuid.DefinedType.FINANCIAL_CURRENCY_TYPE, "Other", "The currency type is other.", SystemGuid.DefinedValue.CURRENCY_TYPE_OTHER, true );

            // NA: Enable AllowGroupSync for Communication List type groups (Fixes #3223).
            Sql( string.Format( "UPDATE [GroupType] SET [AllowGroupSync] = 1 WHERE [AllowGroupSync] = 0 AND [Guid] = '{0}'", Rock.SystemGuid.GroupType.GROUPTYPE_COMMUNICATIONLIST ) );

            // NA: Make Personal Device Platform Defined Values IsSystem=true
            // Fix Personal Device Platform Defined Values to IsSystem=true
            Sql( string.Format( "UPDATE [DefinedValue] SET [IsSystem] = 1 WHERE [IsSystem] = 0 AND [Guid] = '{0}'", Rock.SystemGuid.DefinedValue.PERSONAL_DEVICE_PLATFORM_ANDROID ) );
            Sql( string.Format( "UPDATE [DefinedValue] SET [IsSystem] = 1 WHERE [IsSystem] = 0 AND [Guid] = '{0}'", Rock.SystemGuid.DefinedValue.PERSONAL_DEVICE_PLATFORM_IOS ) );

            // NA: Fix FrontPorch Device Platform Defined Values IsSystem=true
            // Fix FrontPorch Device Platform Defined Values IsSystem=true
            // NOTE: These do not have a well-known GUIDs so we will target them via their value and description.
            Sql( string.Format( "UPDATE [DefinedValue] SET [IsSystem] = 1 WHERE [IsSystem] = 0 AND [DefinedTypeId] = ( SELECT [Id] FROM [DefinedType] WHERE [Guid] = '{0}' ) AND [Value] = 'Windows' AND [Description] = 'A Windows device'", Rock.SystemGuid.DefinedType.PERSONAL_DEVICE_PLATFORM ) );
            Sql( string.Format( "UPDATE [DefinedValue] SET [IsSystem] = 1 WHERE [IsSystem] = 0 AND [DefinedTypeId] = ( SELECT [Id] FROM [DefinedType] WHERE [Guid] = '{0}' ) AND [Value] = 'Mac' AND [Description] = 'A Macintosh device'", Rock.SystemGuid.DefinedType.PERSONAL_DEVICE_PLATFORM ) );
        }

        /// <summary>
        /// GJ: Update Wordcloud Shortcode
        /// This migration was originally in 055_MigrationRollupsForV8_3
        /// </summary>
        private void UpdateWordcloudShortcode()
        {
            Sql( @"UPDATE [LavaShortCode] 
SET [Markup] = '{% javascript id:''d3-layout-cloud'' url:''~/Scripts/d3-cloud/d3.layout.cloud.js'' %}{% endjavascript %}
{% javascript id:''d3-min'' url:''~/Scripts/d3-cloud/d3.min.js'' %}{% endjavascript %}

<div id=""{{ uniqueid }}"" style=""width: {{ width }}; height: {{ height }};""></div>

{%- assign anglecount = anglecount | Trim -%}
{%- assign anglemin = anglemin | Trim -%}
{%- assign anglemax = anglemax | Trim -%}

{% javascript disableanonymousfunction:''true'' %}
    $( document ).ready(function() {
        Rock.controls.wordcloud.initialize({
            inputTextId: ''hf-{{ uniqueid }}'',
            visId: ''{{ uniqueid }}'',
            width: ''{{ width }}'',
            height: ''{{ height }}'',
            fontName: ''{{ fontname }}'',
            maxWords: {{ maxwords }},
            scaleName: ''{{ scalename }}'',
            spiralName: ''{{ spiralname}}'',
            colors: [ ''{{ colors | Replace:'','',""'',''"" }}''],
            {%- if anglecount != '''' %}
            anglecount: {{ anglecount }}{%- if anglemin != '''' or anglemax != '''' -%},{%- endif -%}
            {%- endif -%}
            {%- if anglemin != '''' %}
            anglemin: {{ anglemin }}{%- if anglemax != '''' -%},{%- endif -%}
            {%- endif -%}
            {%- if anglemax != '''' %}
            anglemax: {{ anglemax }}
            {%- endif -%}
        });
    });
{% endjavascript %}

<input type=""hidden"" id=""hf-{{ uniqueid }}"" value=""{{ blockContent }}"" />'
select * from LavaShortcode
WHERE [Guid] = '4B6452EF-6FEA-4A66-9FB9-1A7CCE82E7A4'
and [Markup] = '{%- javascript url:''https://cdnjs.cloudflare.com/ajax/libs/jarallax/1.9.2/jarallax.min.js'' id:''jarallax-shortcode'' -%}{%- endjavascript -%}
{% if videourl != '''' -%}
    {%- javascript url:''https://cdnjs.cloudflare.com/ajax/libs/jarallax/1.9.2/jarallax-video.min.js'' id:''jarallax-video-shortcode'' -%}{%- endjavascript -%}
{% endif -%}

{% assign id = uniqueid -%} 
{% assign bodyZindex = zindex | Plus:1 -%}

{% assign speed = speed | AsInteger %}

{% if speed > 0 -%}
    {% assign speed = speed | Times:''.01'' -%}
    {% assign speed = speed | Plus:''1'' -%}
{% elseif speed == 0 -%}
    {% assign speed = 1 -%}
{% else -%}
    {% assign speed = speed | Times:''.02'' -%}
    {% assign speed = speed | Plus:''1'' -%}
{% endif -%}


 
{% if videourl != ''''- %}
    <div id=""{{ id }}"" class=""jarallax"" data-jarallax-video=""{{ videourl }}"" data-type=""{{ type }}"" data-speed=""{{ speed }}"" data-img-position=""{{ position }}"" data-object-position=""{{ position }}"" data-background-position=""{{ position }}"" data-zindex=""{{ bodyZindex }}"" data-no-android=""{{ noandroid }}"" data-no-ios=""{{ noios }}"">
{% else- %} 
    <div id=""{{ id }}"" data-jarallax class=""jarallax"" data-type=""{{ type }}"" data-speed=""{{ speed }}"" data-img-position=""{{ position }}"" data-object-position=""{{ position }}"" data-background-position=""{{ position }}"" data-zindex=""{{ bodyZindex }}"" data-no-android=""{{ noandroid }}"" data-no-ios=""{{ noios }}"">
        <img class=""jarallax-img"" src=""{{ image }}"" alt="""">
{% endif -%}

        {% if blockContent != '''' -%}
            <div class=""parallax-content"">
                {{ blockContent }}
            </div>
        {% else- %}
            {{ blockContent }}
        {% endif -%}
    </div>

{% stylesheet %}
#{{ id }} {
    /* eventually going to change the height using media queries with mixins using sass, and then include only the classes I want for certain parallaxes */
    min-height: {{ height }};
    background: transparent;
    position: relative;
    z-index: 0;
}

#{{ id }} .jarallax-img {
    position: absolute;
    object-fit: cover;
    /* support for plugin https://github.com/bfred-it/object-fit-images */
    font-family: ''object-fit: cover;'';
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    z-index: -1;
}

#{{ id }} .parallax-content{
    display: inline-block;
    margin: {{ contentpadding }};
    color: {{ contentcolor }};
    text-align: {{ contentalign }};
	width: 100%;
}
{% endstylesheet %}'" );
        }

        /// <summary>
        /// GP: Enable NCOA
        /// This migration was originally in 056_MigrationRollupsForV8_4
        /// </summary>
        private void EnableNCOA()
        {
            // 
            Sql( @"
                DELETE FROM [dbo].[Auth] WHERE [Guid] = '8628653e-f4d8-42b4-4793-38647fada3f1'
                
                UPDATE [dbo].[ServiceJob]
                SET [IsActive] = 1, [Description] = 'Job that gets National Change of Address (NCOA) data'
                WHERE Guid = 'D2D6EA6C-F94A-39A0-481B-A23D08B887D6'" );
        }

        /// <summary>
        /// Migration originally inlcluded in 058_CheckinRegistration
        /// </summary>
        private void CheckinRegistration()
        {
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.GroupType", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "GroupTypePurposeValueId", "142", "Display Alternate ID Field for Adults", "", 37, "False", "0C85A243-51D5-4372-BDA7-D07D437CD765", "core_checkin_registration_DisplayAlternateIdFieldforAdults" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.GroupType", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "GroupTypePurposeValueId", "142", "Display Alternate ID Field for Children", "", 38, "False", "6F1267DC-2707-40B4-9742-BD24616E8871", "core_checkin_registration_DisplayAlternateIdFieldforChildren" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.GroupType", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "GroupTypePurposeValueId", "142", "Required Attributes for Adults", "", 39, "", "57EC6498-6CCF-4616-96E2-A82426361540", "core_checkin_registration_RequiredAttributesforAdults" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.GroupType", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "GroupTypePurposeValueId", "142", "Optional Attributes for Adults", "", 40, "", "9A889765-BD71-43C8-9D18-F7C59EE32384", "core_checkin_registration_OptionalAttributesforAdults" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.GroupType", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "GroupTypePurposeValueId", "142", "Required Attributes for Children", "", 41, "", "09DA6B34-9430-455F-AA73-9A1CBA61B308", "core_checkin_registration_RequiredAttributesforChildren" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.GroupType", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "GroupTypePurposeValueId", "142", "Optional Attributes for Children", "", 42, "", "ADCBD9E1-E058-4591-AFE2-D671182F4744", "core_checkin_registration_OptionalAttributesforChildren" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.GroupType", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "GroupTypePurposeValueId", "142", "Required Attributes for Families", "", 43, "", "DD198B9A-075E-434C-92DB-413411C1AD77", "core_checkin_registration_RequiredAttributesforFamilies" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.GroupType", "99B090AA-4D7E-46D8-B393-BF945EA1BA8B", "GroupTypePurposeValueId", "142", "Optional Attributes for Families", "", 44, "", "3967BF30-0550-4626-975F-DEED0BDEA479", "core_checkin_registration_OptionalAttributesforFamilies" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.GroupType", "1D50399B-0D6E-480B-A71A-E7BD37DD83F0", "GroupTypePurposeValueId", "142", "Add Family Workflow Types", "The workflow types that should be launched when the family is saved.", 45, "", "EF830646-2102-4925-B157-1CCB49D750F1", "core_checkin_registration_AddFamilyWorkflowTypes" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.GroupType", "1D50399B-0D6E-480B-A71A-E7BD37DD83F0", "GroupTypePurposeValueId", "142", "Add Person Workflow Types", "", 46, "", "87FBEE5A-55DC-4A3D-A864-6882C34498AD", "core_checkin_registration_AddPersonWorkflowTypes" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.GroupType", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "GroupTypePurposeValueId", "142", "Enable Check-in After Registration", "", 47, "True", "1A209C15-565A-4994-B4EA-F2CC04B2A39C", "core_checkin_registration_EnableCheckInAfterRegistration" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.GroupType", "BD0D9B57-2A41-4490-89FF-F01DAB7D4904", "GroupTypePurposeValueId", "142", "Known Relationship Types", "The known relationships to display in the child’s ‘Relationship to Adult’ field.", 48, "9,0", "93176010-C4E4-4D0A-99C6-D7E73E76E920", "core_checkin_registration_KnownRelationshipTypes" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.GroupType", "BD0D9B57-2A41-4490-89FF-F01DAB7D4904", "GroupTypePurposeValueId", "142", "Same Family Known Relationship Types", "Of the known relationships defined by Relationship to Adult, which should be used to place the child in the family with the adults.", 49, "0", "AD151CDD-FDDE-4F58-AD28-CCAAE1B27F53", "core_checkin_registration_SameFamilyKnownRelationshipTypes" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.GroupType", "BD0D9B57-2A41-4490-89FF-F01DAB7D4904", "GroupTypePurposeValueId", "142", "Can Check-in Known Relationship Types", "The known relationships that will place the child in a separate family with a ‘Can Check-in’ relationship back to the person.", 50, "9", "CA8F82B4-66F2-4A2E-9711-3E866F73A4A2", "core_checkin_registration_CanCheckInKnownRelationshipTypes" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.GroupType", "59D5A94C-94A0-4630-B80A-BB25697D74C7", "GroupTypePurposeValueId", "142", "Default Person Connection Status", "", 1003, "b91ba046-bc1e-400c-b85d-638c1f4e0ce2", "6869D3CC-2518-4F56-BB55-5E9B128AFB68", "core_checkin_registration_DefaultPersonConnectionStatus" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.GroupType", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "GroupTypePurposeValueId", "142", "Start Lava Template", "The lava template to use when rendering the Start button on the Welcome Block.", 1004, @"<div class='checkin-search-actions checkin-start'>
            {% if RegistrationModeEnabled == true %}
                <a class='btn btn-primary btn-checkin js-start-button'>
                  <span>{{ CheckinButtonText }}</span>
                </a>
            {% else %}
                <a class='btn btn-primary btn-checkin js-start-button'>
                  <span>{{ CheckinButtonText }}</span>
               </a>
            {% endif %}
            </div>
            ", "5F242D2A-FD01-4508-9F4C-ED01124309E7", "core_checkin_StartLavaTemplate" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.GroupType", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "GroupTypePurposeValueId", "142", "Family Select Template", "The lava template to use when rendering each family button on the Family Select block.", 1005, @"<a class='btn btn-primary btn-large btn-block btn-checkin-select'>
            {% if RegistrationModeEnabled == true %}
                {{ Family.Group.Name }}<span class='checkin-sub-title'>{{ Family.SubCaption }}</span>
            {% else %}
                {{ Family.Group.Name }}<span class='checkin-sub-title'>{{ Family.SubCaption }}</span>
            {% endif %}
            </a>", "3ABA00E7-2C55-434B-BA1F-C392CC9C9D90", "core_checkin_FamilyLavaTemplate" );
            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.GroupType", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "GroupTypePurposeValueId", "142", "Success Template", "", 1006, @"<ol class=""checkin-summary checkin-body-container"">

            {% for checkinResult in CheckinResultList %}
                {% if RegistrationModeEnabled == true %}
                    <li>{{ checkinResult.DetailMessage }}</li>
                {% else %}
                    <li>{{ checkinResult.DetailMessage }}</li>
                {% endif %}
            {% endfor %}

            {% comment %}Display any error messages from the label printer{% endcomment %}
            {% for message in ZebraPrintMessageList %}
                <br/>{{ message }}
            {% endfor %}

            </ol>", "F5BA6DCC-0A4D-4616-871D-ECBA7082C45F", "core_checkin_SuccessLavaTemplate" );


            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.Device", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "DeviceTypeValueId", "41", "Registration Mode", "Kiosk serves the role of enrolling new families. This enables Add Family and Edit Family features during the check-in process.", 1002, "False", "05F72E44-9D94-4C97-9458-13B038EDEAE3", "core_device_RegistrationMode" );

            // core_device_RegistrationMode
            RockMigrationHelper.UpdateAttributeQualifier( "05F72E44-9D94-4C97-9458-13B038EDEAE3", "falsetext", @"No", "E04BB261-E10F-4C37-A66D-63072393A2DC" );

            // core_device_RegistrationMode
            RockMigrationHelper.UpdateAttributeQualifier( "05F72E44-9D94-4C97-9458-13B038EDEAE3", "truetext", @"Yes", "C1DC97E7-356A-401D-9CC7-95C32862A840" );


            // Update QualifierValues to non-hardcoded IDs based on Guid
            Sql( @"
            DECLARE @CheckInTemplatePurposeId INT = (
                    SELECT TOP 1 Id
                    FROM DefinedValue
                    WHERE [Guid] = '4A406CB0-495B-4795-B788-52BDFDE00B01'
                    )

            UPDATE Attribute
            SET EntityTypeQualifierValue = @CheckInTemplatePurposeId
            WHERE [Guid] IN (
                    '0C85A243-51D5-4372-BDA7-D07D437CD765'
                    ,'6F1267DC-2707-40B4-9742-BD24616E8871'
                    ,'57EC6498-6CCF-4616-96E2-A82426361540'
                    ,'09DA6B34-9430-455F-AA73-9A1CBA61B308'
                    ,'ADCBD9E1-E058-4591-AFE2-D671182F4744'
                    ,'EF830646-2102-4925-B157-1CCB49D750F1'
                    ,'1A209C15-565A-4994-B4EA-F2CC04B2A39C'
                    ,'93176010-C4E4-4D0A-99C6-D7E73E76E920'
                    ,'AD151CDD-FDDE-4F58-AD28-CCAAE1B27F53'
                    ,'CA8F82B4-66F2-4A2E-9711-3E866F73A4A2'
                    ,'9A889765-BD71-43C8-9D18-F7C59EE32384'
                    ,'3967BF30-0550-4626-975F-DEED0BDEA479'
                    ,'DD198B9A-075E-434C-92DB-413411C1AD77'
                    ,'87FBEE5A-55DC-4A3D-A864-6882C34498AD'
                    ,'6869D3CC-2518-4F56-BB55-5E9B128AFB68'
                    ,'5F242D2A-FD01-4508-9F4C-ED01124309E7'
                    ,'3ABA00E7-2C55-434B-BA1F-C392CC9C9D90'
                    ,'F5BA6DCC-0A4D-4616-871D-ECBA7082C45F'  
                    )
            " );


            // core_checkin_registration_DisplayAlternateIdFieldforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "0C85A243-51D5-4372-BDA7-D07D437CD765", "falsetext", @"No", "5F6C4098-DE5A-4DFB-A154-0ACB53055C53" );

            // core_checkin_registration_DisplayAlternateIdFieldforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "0C85A243-51D5-4372-BDA7-D07D437CD765", "truetext", @"Yes", "2EF79907-A63D-40EB-A014-C5698CF48AF9" );

            // core_checkin_registration_DisplayAlternateIdFieldforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "6F1267DC-2707-40B4-9742-BD24616E8871", "falsetext", @"No", "40916EF5-BA5C-4620-BF55-F26934FB2D9C" );

            // core_checkin_registration_DisplayAlternateIdFieldforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "6F1267DC-2707-40B4-9742-BD24616E8871", "truetext", @"Yes", "57E9F65B-8E56-4B4E-9839-7CDE76DCEC1D" );

            // core_checkin_registration_RequiredAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "57EC6498-6CCF-4616-96E2-A82426361540", "allowmultiple", @"True", "9E9BC7FE-5F05-4F1C-B38D-265C18040E58" );

            // core_checkin_registration_RequiredAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "57EC6498-6CCF-4616-96E2-A82426361540", "entitytype", @"72657ed8-d16e-492e-ac12-144c5e7567e7", "EEC9B75D-0F7C-4C5B-BD9E-280F31339C34" );

            // core_checkin_registration_RequiredAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "57EC6498-6CCF-4616-96E2-A82426361540", "qualifierColumn", @"", "35FAE6B2-543D-44C6-B7A6-12879CC1BACF" );

            // core_checkin_registration_RequiredAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "57EC6498-6CCF-4616-96E2-A82426361540", "qualifierValue", @"", "0DB21AD6-4ADE-42E1-9B6B-91476DAC072E" );

            // core_checkin_registration_OptionalAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "9A889765-BD71-43C8-9D18-F7C59EE32384", "allowmultiple", @"True", "7118CDE5-B6A6-49FB-BF7B-6258302D8C50" );

            // core_checkin_registration_OptionalAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "9A889765-BD71-43C8-9D18-F7C59EE32384", "entitytype", @"72657ed8-d16e-492e-ac12-144c5e7567e7", "AC455AEF-4CEC-4F4C-B132-21348C5809AB" );

            // core_checkin_registration_OptionalAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "9A889765-BD71-43C8-9D18-F7C59EE32384", "qualifierColumn", @"", "DFDCBD88-449F-4089-8B41-98573568A5A8" );

            // core_checkin_registration_OptionalAttributesforAdults
            RockMigrationHelper.UpdateAttributeQualifier( "9A889765-BD71-43C8-9D18-F7C59EE32384", "qualifierValue", @"", "65519AB4-0FF5-40F2-AFE0-7B20B00EDBFF" );

            // core_checkin_registration_RequiredAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "09DA6B34-9430-455F-AA73-9A1CBA61B308", "allowmultiple", @"True", "97663044-E479-489C-8B86-A6BB0790EAFB" );

            // core_checkin_registration_RequiredAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "09DA6B34-9430-455F-AA73-9A1CBA61B308", "entitytype", @"72657ed8-d16e-492e-ac12-144c5e7567e7", "ED42A953-1E03-4E5A-A5F5-475D78562102" );

            // core_checkin_registration_RequiredAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "09DA6B34-9430-455F-AA73-9A1CBA61B308", "qualifierColumn", @"", "3CDA3CA4-1AB5-446D-8F00-BC635FECFDF0" );

            // core_checkin_registration_RequiredAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "09DA6B34-9430-455F-AA73-9A1CBA61B308", "qualifierValue", @"", "0F502F17-A2A8-47FC-87A8-C8C1849D4FA5" );

            // core_checkin_registration_OptionalAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "ADCBD9E1-E058-4591-AFE2-D671182F4744", "allowmultiple", @"True", "268A4D1D-B2C5-4849-A7F0-6671FBB2D3D7" );

            // core_checkin_registration_OptionalAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "ADCBD9E1-E058-4591-AFE2-D671182F4744", "entitytype", @"72657ed8-d16e-492e-ac12-144c5e7567e7", "9CA33C84-5332-42A7-8E24-BAAD9D432CA2" );

            // core_checkin_registration_OptionalAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "ADCBD9E1-E058-4591-AFE2-D671182F4744", "qualifierColumn", @"", "810B1798-5503-4C8F-B1E9-B5D9F38DA75C" );

            // core_checkin_registration_OptionalAttributesforChildren
            RockMigrationHelper.UpdateAttributeQualifier( "ADCBD9E1-E058-4591-AFE2-D671182F4744", "qualifierValue", @"", "449EB382-04EB-4C98-9F61-D157DEEE6111" );

            // core_checkin_registration_RequiredAttributesforFamilies
            RockMigrationHelper.UpdateAttributeQualifier( "DD198B9A-075E-434C-92DB-413411C1AD77", "allowmultiple", @"True", "96528D05-7C42-47A6-A562-E12CA0A5FB3E" );

            // core_checkin_registration_RequiredAttributesforFamilies
            RockMigrationHelper.UpdateAttributeQualifier( "DD198B9A-075E-434C-92DB-413411C1AD77", "entitytype", @"9bbfda11-0d22-40d5-902f-60adfbc88987", "6717B74A-67C2-47B6-84A2-B3E809410B1E" );

            // core_checkin_registration_RequiredAttributesforFamilies
            RockMigrationHelper.UpdateAttributeQualifier( "DD198B9A-075E-434C-92DB-413411C1AD77", "qualifierColumn", @"GroupTypeId", "3CF45687-AA5D-4381-8A99-FC7F81775B67" );

            // core_checkin_registration_RequiredAttributesforFamilies
            RockMigrationHelper.UpdateAttributeQualifier( "DD198B9A-075E-434C-92DB-413411C1AD77", "qualifierValue", @"10", "92BC0C04-C328-4CA9-B7AA-AEAF46D11BF2" );

            // core_checkin_registration_OptionalAttributesforFamilies
            RockMigrationHelper.UpdateAttributeQualifier( "3967BF30-0550-4626-975F-DEED0BDEA479", "allowmultiple", @"True", "5ED1E88F-FF42-42A4-9180-1731C1D6331C" );

            // core_checkin_registration_OptionalAttributesforFamilies
            RockMigrationHelper.UpdateAttributeQualifier( "3967BF30-0550-4626-975F-DEED0BDEA479", "entitytype", @"9bbfda11-0d22-40d5-902f-60adfbc88987", "6C3CB18A-ACB5-4FD3-901F-CD42772BD8F3" );

            // core_checkin_registration_OptionalAttributesforFamilies
            RockMigrationHelper.UpdateAttributeQualifier( "3967BF30-0550-4626-975F-DEED0BDEA479", "qualifierColumn", @"GroupTypeId", "A537A676-60EC-4F62-8081-786DBBB61F69" );

            // core_checkin_registration_OptionalAttributesforFamilies
            RockMigrationHelper.UpdateAttributeQualifier( "3967BF30-0550-4626-975F-DEED0BDEA479", "qualifierValue", @"10", "AB7D785E-96E2-44E3-8518-A4C53C6961EF" );

            // core_checkin_registration_EnableCheckInAfterRegistration
            RockMigrationHelper.UpdateAttributeQualifier( "1A209C15-565A-4994-B4EA-F2CC04B2A39C", "falsetext", @"No", "E9A5F505-C477-4F26-BC47-A077E2AD0C4B" );

            // core_checkin_registration_EnableCheckInAfterRegistration
            RockMigrationHelper.UpdateAttributeQualifier( "1A209C15-565A-4994-B4EA-F2CC04B2A39C", "truetext", @"Yes", "2364692B-BFAD-4F71-9575-3ED2C64765E0" );

            // core_checkin_registration_KnownRelationshipTypes
            RockMigrationHelper.UpdateAttributeQualifier( "93176010-C4E4-4D0A-99C6-D7E73E76E920", "enhancedselection", @"True", "89F96B1F-C9BC-42BB-AED7-34C91A1C224C" );

            // core_checkin_registration_KnownRelationshipTypes
            RockMigrationHelper.UpdateAttributeQualifier( "93176010-C4E4-4D0A-99C6-D7E73E76E920", "values", @"SELECT 
            	    R.[Id] AS [Value],
            	    R.[Name] AS [Text]
                FROM [GroupType] T
                INNER JOIN [GroupTypeRole] R ON R.[GroupTypeId] = T.[Id]
                WHERE T.[Guid] = 'E0C5A0E2-B7B3-4EF4-820D-BBF7F9A374EF'
                AND R.[Name] <> 'Child'
                UNION ALL
                SELECT 0, 'Child'
                ORDER BY [Text]", "365BEA08-BEBE-4E6E-9EA4-0141EDFE91CB" );

            // core_checkin_registration_SameFamilyKnownRelationshipTypes
            RockMigrationHelper.UpdateAttributeQualifier( "AD151CDD-FDDE-4F58-AD28-CCAAE1B27F53", "enhancedselection", @"True", "4E6AAE71-785D-444D-B1EF-8B915A019030" );

            // core_checkin_registration_SameFamilyKnownRelationshipTypes
            RockMigrationHelper.UpdateAttributeQualifier( "AD151CDD-FDDE-4F58-AD28-CCAAE1B27F53", "values", @"SELECT 
            	    R.[Id] AS [Value],
            	    R.[Name] AS [Text]
                FROM [GroupType] T
                INNER JOIN [GroupTypeRole] R ON R.[GroupTypeId] = T.[Id]
                WHERE T.[Guid] = 'E0C5A0E2-B7B3-4EF4-820D-BBF7F9A374EF'
                AND R.[Name] <> 'Child'
                UNION ALL
                SELECT 0, 'Child'
                ORDER BY [Text]", "75C942EC-F6F1-4B62-BFAE-321FA9E545D6" );

            // core_checkin_registration_CanCheckInKnownRelationshipTypes
            RockMigrationHelper.UpdateAttributeQualifier( "CA8F82B4-66F2-4A2E-9711-3E866F73A4A2", "enhancedselection", @"True", "FC1AACFF-A32F-4D8B-A78C-02A4D5D994D4" );

            // core_checkin_registration_CanCheckInKnownRelationshipTypes
            RockMigrationHelper.UpdateAttributeQualifier( "CA8F82B4-66F2-4A2E-9711-3E866F73A4A2", "values", @"SELECT 
                    R.[Id] AS [Value],
                    R.[Name] AS [Text]
                FROM [GroupType] T
                INNER JOIN [GroupTypeRole] R ON R.[GroupTypeId] = T.[Id]
                WHERE T.[Guid] = 'E0C5A0E2-B7B3-4EF4-820D-BBF7F9A374EF'
                AND R.[Name] <> 'Child'
                UNION ALL
                SELECT 0, 'Child'
                ORDER BY [Text]", "8B055917-4E83-435E-9C1D-605245AA00BB" );


            // core_checkin_registration_DefaultPersonConnectionStatus
            RockMigrationHelper.UpdateAttributeQualifier( "6869D3CC-2518-4F56-BB55-5E9B128AFB68", "allowmultiple", @"False", "C34B3347-23EF-4CAD-92E5-5209D09F7F21" );

            // core_checkin_registration_DefaultPersonConnectionStatus
            RockMigrationHelper.UpdateAttributeQualifier( "6869D3CC-2518-4F56-BB55-5E9B128AFB68", "definedtype", @"4", "7897A05B-3E03-47CD-A4B5-389ABDEECD2A" );

            // core_checkin_registration_DefaultPersonConnectionStatus
            RockMigrationHelper.UpdateAttributeQualifier( "6869D3CC-2518-4F56-BB55-5E9B128AFB68", "displaydescription", @"False", "85B6D721-E1D6-4971-BFC1-572A93CC1404" );

            // core_checkin_registration_DefaultPersonConnectionStatus
            RockMigrationHelper.UpdateAttributeQualifier( "6869D3CC-2518-4F56-BB55-5E9B128AFB68", "enhancedselection", @"False", "CFD310E9-6FA8-4971-8DFF-76CA60030968" );

            // core_checkin_registration_DefaultPersonConnectionStatus
            RockMigrationHelper.UpdateAttributeQualifier( "6869D3CC-2518-4F56-BB55-5E9B128AFB68", "includeInactive", @"False", "5CCED4C9-EDC4-4292-991D-6865E4C96EB7" );


            // core_checkin_StartLavaTemplate
            RockMigrationHelper.UpdateAttributeQualifier( "5F242D2A-FD01-4508-9F4C-ED01124309E7", "editorHeight", @"", "52CD97C5-89C0-46AA-9643-93BAFE1048CC" );

            // core_checkin_StartLavaTemplate
            RockMigrationHelper.UpdateAttributeQualifier( "5F242D2A-FD01-4508-9F4C-ED01124309E7", "editorMode", @"3", "9454CEDB-3975-4F30-A386-DC58C53AEEEF" );

            // core_checkin_StartLavaTemplate
            RockMigrationHelper.UpdateAttributeQualifier( "5F242D2A-FD01-4508-9F4C-ED01124309E7", "editorTheme", @"0", "EA0D3D09-26BB-4248-918C-EBB2617FEE99" );

            // core_checkin_FamilyLavaTemplate
            RockMigrationHelper.UpdateAttributeQualifier( "3ABA00E7-2C55-434B-BA1F-C392CC9C9D90", "editorHeight", @"", "6628E68A-E4AF-4935-B3FF-C63DDD009D06" );

            // core_checkin_FamilyLavaTemplate
            RockMigrationHelper.UpdateAttributeQualifier( "3ABA00E7-2C55-434B-BA1F-C392CC9C9D90", "editorMode", @"3", "DFE260FF-1453-4398-8788-31A337DC476C" );

            // core_checkin_FamilyLavaTemplate
            RockMigrationHelper.UpdateAttributeQualifier( "3ABA00E7-2C55-434B-BA1F-C392CC9C9D90", "editorTheme", @"0", "386481B8-6CC4-4603-8B52-F2CE14B42095" );

            // core_checkin_SuccessLavaTemplate
            RockMigrationHelper.UpdateAttributeQualifier( "F5BA6DCC-0A4D-4616-871D-ECBA7082C45F", "editorHeight", @"", "0DBF3708-7CD9-4D0E-8C68-5177AC8E6ADC" );

            // core_checkin_SuccessLavaTemplate
            RockMigrationHelper.UpdateAttributeQualifier( "F5BA6DCC-0A4D-4616-871D-ECBA7082C45F", "editorMode", @"3", "48BD2454-304F-416B-AC49-2398F47667C9" );

            // core_checkin_SuccessLavaTemplate
            RockMigrationHelper.UpdateAttributeQualifier( "F5BA6DCC-0A4D-4616-871D-ECBA7082C45F", "editorTheme", @"0", "81AB2246-8592-47D2-8090-AB392BC271E3" );


            /* New Checkin Blocks */
            RockMigrationHelper.UpdateBlockType( "Edit Family", "Block to Add or Edit a Family during the Check-in Process.", "~/Blocks/CheckIn/EditFamily.ascx", "Check-in", "06DF448A-684E-4B64-8E1B-EA1727BA9233" );

            // Attrib for BlockType: Edit Family:Workflow Activity
            RockMigrationHelper.UpdateBlockTypeAttribute( "06DF448A-684E-4B64-8E1B-EA1727BA9233", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Workflow Activity", "WorkflowActivity", "", @"The name of the workflow activity to run on selection.", 1, @"", "9B1B49A1-716D-4B5C-A75E-D39B681207AB" );

            // Attrib for BlockType: Edit Family:Home Page
            RockMigrationHelper.UpdateBlockTypeAttribute( "06DF448A-684E-4B64-8E1B-EA1727BA9233", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Home Page", "HomePage", "", @"", 2, @"", "808EA130-EDED-45FD-9683-A5A26859128F" );
            // Attrib for BlockType: Edit Family:Previous Page
            RockMigrationHelper.UpdateBlockTypeAttribute( "06DF448A-684E-4B64-8E1B-EA1727BA9233", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Previous Page", "PreviousPage", "", @"", 3, @"", "FC3382DD-647F-4C2F-A06F-3A0C274B8B95" );
            // Attrib for BlockType: Edit Family:Next Page
            RockMigrationHelper.UpdateBlockTypeAttribute( "06DF448A-684E-4B64-8E1B-EA1727BA9233", "BD53F9C9-EBA9-4D3F-82EA-DE5DD34A8108", "Next Page", "NextPage", "", @"", 4, @"", "B9F99590-3E58-4746-9884-14D2223D00F8" );
            // Attrib for BlockType: Edit Family:Workflow Type
            RockMigrationHelper.UpdateBlockTypeAttribute( "06DF448A-684E-4B64-8E1B-EA1727BA9233", "46A03F59-55D3-4ACE-ADD5-B4642225DD20", "Workflow Type", "WorkflowType", "", @"The workflow type to activate for check-in", 0, @"", "C7C8C51E-B5A0-49A5-96F3-CB23BB5F81AB" );

            // Add Block to Page: Search, Site: Rock Check-in
            RockMigrationHelper.AddBlock( true, "D47858C0-0E6E-46DC-AE99-8EC84BA5F45F", "", "06DF448A-684E-4B64-8E1B-EA1727BA9233", "Add Family", "Main", @"", @"", 2, "7FBE00BD-7A4E-4F2D-89F1-D62348F4F146" );
            // Attrib Value for Block:Edit Family, Attribute:Workflow Activity Page: Search, Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( true, "7FBE00BD-7A4E-4F2D-89F1-D62348F4F146", "9B1B49A1-716D-4B5C-A75E-D39B681207AB", @"Family Search" );
            // Attrib Value for Block:Edit Family, Attribute:Workflow Type Page: Search, Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( true, "7FBE00BD-7A4E-4F2D-89F1-D62348F4F146", "C7C8C51E-B5A0-49A5-96F3-CB23BB5F81AB", Rock.SystemGuid.WorkflowType.UNATTENDED_CHECKIN );
            // Attrib Value for Block:Edit Family, Attribute:Home Page Page: Search, Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( true, "7FBE00BD-7A4E-4F2D-89F1-D62348F4F146", "808EA130-EDED-45FD-9683-A5A26859128F", Rock.SystemGuid.Page.WELCOME );

            // Add Block to Page: Family Select, Site: Rock Check-in
            RockMigrationHelper.AddBlock( true, "10C97379-F719-4ACB-B8C6-651957B660A4", "", "06DF448A-684E-4B64-8E1B-EA1727BA9233", "Add Family", "Main", @"", @"", 2, "829998AD-2992-4A11-932F-5C3AE5B09895" );
            // Attrib Value for Block:Edit Family, Attribute:Workflow Activity Page: Family Select, Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( true, "829998AD-2992-4A11-932F-5C3AE5B09895", "9B1B49A1-716D-4B5C-A75E-D39B681207AB", @"Family Search" );
            // Attrib Value for Block:Edit Family, Attribute:Workflow Type Page: Family Select, Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( true, "829998AD-2992-4A11-932F-5C3AE5B09895", "C7C8C51E-B5A0-49A5-96F3-CB23BB5F81AB", Rock.SystemGuid.WorkflowType.UNATTENDED_CHECKIN );
            // Attrib Value for Block:Edit Family, Attribute:Home Page Page: Family Select, Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( true, "829998AD-2992-4A11-932F-5C3AE5B098956", "808EA130-EDED-45FD-9683-A5A26859128F", Rock.SystemGuid.Page.WELCOME );

            // Add Block to Page: Person Select, Site: Rock Check-in
            RockMigrationHelper.AddBlock( true, "BB8CF87F-680F-48F9-9147-F4951E033D17", "", "06DF448A-684E-4B64-8E1B-EA1727BA9233", "Edit Family", "Main", @"", @"", 2, "5E00309E-EC0D-4B99-A1C7-FD644361E5DD" );
            // Attrib Value for Block:Edit Family, Attribute:Workflow Activity Page: Person Select, Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( true, "5E00309E-EC0D-4B99-A1C7-FD644361E5DD", "9B1B49A1-716D-4B5C-A75E-D39B681207AB", @"Person Search" );
            // Attrib Value for Block:Edit Family, Attribute:Workflow Type Page: Person Select, Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( true, "5E00309E-EC0D-4B99-A1C7-FD644361E5DD", "C7C8C51E-B5A0-49A5-96F3-CB23BB5F81AB", Rock.SystemGuid.WorkflowType.UNATTENDED_CHECKIN );
            // Attrib Value for Block:Edit Family, Attribute:Home Page Page: Person Select, Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( true, "5E00309E-EC0D-4B99-A1C7-FD644361E5DD", "808EA130-EDED-45FD-9683-A5A26859128F", Rock.SystemGuid.Page.WELCOME );

            // Add Block to Page: Person Select (Family Check-in), Site: Rock Check-in
            RockMigrationHelper.AddBlock( true, "D14154BA-2F2C-41C3-B380-F833252CBB13", "", "06DF448A-684E-4B64-8E1B-EA1727BA9233", "Edit Family", "Main", @"", @"", 2, "07BC8F00-2925-4CDC-8F9E-DB431B822770" );
            // Attrib Value for Block:Edit Family, Attribute:Workflow Activity Page: Person Select (Family Check-in), Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( true, "07BC8F00-2925-4CDC-8F9E-DB431B822770", "9B1B49A1-716D-4B5C-A75E-D39B681207AB", @"Person Search" );
            // Attrib Value for Block:Edit Family, Attribute:Workflow Type Page: Person Select (Family Check-in), Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( true, "07BC8F00-2925-4CDC-8F9E-DB431B822770", "C7C8C51E-B5A0-49A5-96F3-CB23BB5F81AB", Rock.SystemGuid.WorkflowType.UNATTENDED_CHECKIN );
            // Attrib Value for Block:Edit Family, Attribute:Home Page Page: Person Select (Family Check-in), Site: Rock Check-in
            RockMigrationHelper.AddBlockAttributeValue( true, "07BC8F00-2925-4CDC-8F9E-DB431B822770", "808EA130-EDED-45FD-9683-A5A26859128F", Rock.SystemGuid.Page.WELCOME );

            // update default Search text for search blocktype
            RockMigrationHelper.UpdateBlockTypeAttribute( "E3A99534-6FD9-49AD-AC52-32D53B2CEDD7", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Title", "Title", "", "Title to display. Use {0} for search type.", 5, @"Search", "837B34E3-D140-44CD-8456-9D222325E42E" );

            // update Search text for search block (if it is still the old default)
            Sql( $@"
                DECLARE @BlockId int = (SELECT [Id] FROM [Block] WHERE [Guid] = '1EF10CB9-DFDC-42CE-9B00-8665050F6B78')
                DECLARE @AttributeId int = (SELECT [Id] FROM [Attribute] WHERE [Guid] = '837B34E3-D140-44CD-8456-9D222325E42E')

                UPDATE [AttributeValue] SET [Value] = 'Search'
                WHERE [AttributeId] = @AttributeId
                    AND [EntityId] = @BlockId
                    AND [Value] = 'Search By {{0}}'" );
        }
    }
}
