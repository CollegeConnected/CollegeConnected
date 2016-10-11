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
}