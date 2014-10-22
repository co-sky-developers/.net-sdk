using System;
using System.Collections.Generic;
using NFleet.Data;

namespace NFleet
{
    static class TypeHelper
    {
        private const string versionPerfix = ";version=";

        private static readonly Dictionary<string, string> supportedTypes = new Dictionary<string, string>
        {
            { UserData.MIMEType, UserData.MIMEType + versionPerfix + UserData.MIMEVersion },
            { ImportData.MIMEType, ImportData.MIMEType + versionPerfix + ImportData.MIMEVersion },
            { ObjectiveValueDataSet.MIMEType, ObjectiveValueDataSet.MIMEType + versionPerfix + ObjectiveValueDataSet.MIMEVersion },
            { PlanData.MIMEType, PlanData.MIMEType + versionPerfix + PlanData.MIMEVersion },
            { RouteData.MIMEType, RouteData.MIMEType + versionPerfix + RouteData.MIMEVersion },
            { RouteEventData.MIMEType, RouteEventData.MIMEType + versionPerfix + RouteEventData.MIMEVersion },
            { RouteEventDataSet.MIMEType, RouteEventDataSet.MIMEType + versionPerfix + RouteEventDataSet.MIMEVersion },
            { RoutingProblemData.MIMEType, RoutingProblemData.MIMEType + versionPerfix + RoutingProblemData.MIMEVersion },
            { RoutingProblemDataSet.MIMEType, RoutingProblemDataSet.MIMEType + versionPerfix + RoutingProblemDataSet.MIMEVersion },
            { RoutingProblemSettingsData.MIMEType, RoutingProblemSettingsData.MIMEType + versionPerfix + RoutingProblemSettingsData.MIMEVersion },
            { TaskData.MIMEType, TaskData.MIMEType + versionPerfix + TaskData.MIMEVersion },
            { TaskDataSet.MIMEType, TaskDataSet.MIMEType + versionPerfix + TaskDataSet.MIMEVersion },
            { TaskEventData.MIMEType, TaskEventData.MIMEType + versionPerfix + TaskEventData.MIMEVersion },
            { TaskEventDataSet.MIMEType, TaskEventDataSet.MIMEType + versionPerfix + TaskEventDataSet.MIMEVersion },
            { VehicleData.MIMEType, VehicleData.MIMEType + versionPerfix + VehicleData.MIMEVersion },
            { VehicleDataSet.MIMEType, VehicleDataSet.MIMEType + versionPerfix + VehicleDataSet.MIMEVersion },
            { VehicleTypeData.MIMEType, VehicleTypeData.MIMEType + versionPerfix + VehicleTypeData.MIMEVersion },

        }; 

        public static string GetSupportedType( string typeString )
        {
            if ( String.IsNullOrEmpty( typeString ) ) return "application/json";
            string[] parts = typeString.Split( ';' );
            string type = parts[0];

            if ( supportedTypes.ContainsKey( type ) )
            {
                return supportedTypes[type];
            }

            return "application/json";
        }
    }
}
