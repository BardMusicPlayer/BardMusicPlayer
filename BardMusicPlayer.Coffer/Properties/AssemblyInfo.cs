using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// In SDK-style projects such as this one, several assembly attributes that were historically
// defined in this file are now automatically added during build and populated with
// values defined in project properties. For details of which attributes are included
// and how to customise this process see: https://aka.ms/assembly-info-properties

// Setting ComVisible to false makes the types in this assembly not visible to COM
// components.  If you need to access a type in this assembly from COM, set the ComVisible
// attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM.
[assembly: Guid("e847734e-d60a-426c-a4eb-a38ec7c49cbc")]

// We need to expose internal methods and classes to the unit test project.
[assembly: InternalsVisibleTo(
    "BardMusicPlayer.Coffer.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100bd3d207230985f"
    + "98a1acfdc9ad781f7137a581b93b62e749d20653a0714bf41b612913be6478ac06a493d379f577"
    + "5501d2f8f76ede7ea3309d9135a99ecd52cb14380e08304f6218a6ce6e99a8534686b8db328fdd"
    + "d632061da8aa02f09ee12e1005df3d36b914ae04c5c14b5557cfc4c13dd007b7314b6815707a65"
    + "9b3cd9d2")]