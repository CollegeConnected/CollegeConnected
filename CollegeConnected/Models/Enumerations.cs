namespace CollegeConnected.Models
{
    public enum ImportStatusEnum
    {
        Ready,

        GettingColumnInformation,

        Running
    }

    public enum ImportProgressTypeEnum
    {
        NotRunning,

        ConvertStart,

        Converted,

        ConvertError,

        ConversionCompleted,

        ImportStart,

        Imported,

        ImportError,

        UploadingResults,

        ImportCompleted,

        Error
    }

    public enum EventStatusEnum
    {
        InProgress,
        Complete
    }

    public enum State
    {
         AL,
        AK,
        AZ,
AR,
CA,
 CO,
 CT,
   DE,
 FL,
 GA,
  HI,
   ID,
    IL,
 IN,
    IA,
  KS,
    KY,
   LA,
   ME,
    MD,
   MA,
    MI,
   MN,
 MS,
    MO,
 MT,
    NE,
  NV,
  NH,
 NJ,
NM,
  NY,
 NC,
  ND,
OH,
 OK,
 OR,
  PA,
   RI,
 SC,
     SD,
   TN,
        TX,
    UT,
 VT,
    VA,
  WA,
WV,
        WI,
 WY
    }

    public enum ConstuentType
    {
        Alumni,
        FalcultyStaff,
        Spouse,
        Other
    }
}