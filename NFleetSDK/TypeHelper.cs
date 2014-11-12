using System;
using System.Collections.Generic;
using NFleet.Data;

namespace NFleet
{
    static class TypeHelper
    {
        private const string versionPrefix = "-";
        private const string versionPostfix = "+json";

        private static readonly Dictionary<string, string> supportedTypes = new Dictionary<string, string>
        {
            { UserData.MIMEType, UserData.MIMEType + versionPrefix + UserData.MIMEVersion + versionPostfix },
            { UserDataSet.MIMEType, UserDataSet.MIMEType + versionPrefix + UserDataSet.MIMEVersion + versionPostfix },
            { ImportData.MIMEType, ImportData.MIMEType + versionPrefix + ImportData.MIMEVersion + versionPostfix },
            { ObjectiveValueDataSet.MIMEType, ObjectiveValueDataSet.MIMEType + versionPrefix + ObjectiveValueDataSet.MIMEVersion + versionPostfix },
            { PlanData.MIMEType, PlanData.MIMEType + versionPrefix + PlanData.MIMEVersion + versionPostfix },
            { RouteData.MIMEType, RouteData.MIMEType + versionPrefix + RouteData.MIMEVersion + versionPostfix },
            { RouteEventData.MIMEType, RouteEventData.MIMEType + versionPrefix + RouteEventData.MIMEVersion + versionPostfix },
            { RouteEventDataSet.MIMEType, RouteEventDataSet.MIMEType + versionPrefix + RouteEventDataSet.MIMEVersion + versionPostfix },
            { RoutingProblemData.MIMEType, RoutingProblemData.MIMEType + versionPrefix + RoutingProblemData.MIMEVersion + versionPostfix },
            { RoutingProblemDataSet.MIMEType, RoutingProblemDataSet.MIMEType + versionPrefix + RoutingProblemDataSet.MIMEVersion + versionPostfix },
            { RoutingProblemSettingsData.MIMEType, RoutingProblemSettingsData.MIMEType + versionPrefix + RoutingProblemSettingsData.MIMEVersion + versionPostfix },
            { TaskData.MIMEType, TaskData.MIMEType + versionPrefix + TaskData.MIMEVersion + versionPostfix },
            { TaskDataSet.MIMEType, TaskDataSet.MIMEType + versionPrefix + TaskDataSet.MIMEVersion + versionPostfix },
            { TaskEventData.MIMEType, TaskEventData.MIMEType + versionPrefix + TaskEventData.MIMEVersion + versionPostfix },
            { TaskEventDataSet.MIMEType, TaskEventDataSet.MIMEType + versionPrefix + TaskEventDataSet.MIMEVersion + versionPostfix },
            { VehicleData.MIMEType, VehicleData.MIMEType + versionPrefix + VehicleData.MIMEVersion + versionPostfix },
            { VehicleDataSet.MIMEType, VehicleDataSet.MIMEType + versionPrefix + VehicleDataSet.MIMEVersion + versionPostfix },
            { VehicleTypeData.MIMEType, VehicleTypeData.MIMEType + versionPrefix + VehicleTypeData.MIMEVersion + versionPostfix },

        }; 

        public static string GetSupportedType( string typeString )
        {
            if ( String.IsNullOrEmpty( typeString ) ) return "application/json";

            string[] parts = typeString.Split( '-' );

            string type = parts[0];

            if ( supportedTypes.ContainsKey( type ) )
            {
                return supportedTypes[type];
            }

            return "application/json";
        }
    }
}
