using System;
using System.IO;
using DIKUArcade.Graphics;

namespace SpaceTaxi_3 {
    public enum TaxiOrientation
    {
        TaxiOrientedLeft,
        TaxiOrientedRight
    };

    public enum TaxiBoosterState
    {
        TaxiBoosterOff,
        TaxiBoosterHorizontal,
        TaxiBoosterUp,
        TaxiBoosterUpAndHorizontal
    }

    public enum CustomerOrientation
    {
        CustomerOrientedLeft,
        CustomerOrientedRight
    };

    public enum CustomerState
    {
        CustomerStateStanding,
        CustomerStateWalking
    };


    public class ImageParser {
        // singleton design pattern
        private static ImageParser instance;

        public static ImageParser GetInstance() {
            return ImageParser.instance ?? (ImageParser.instance = new ImageParser());
        }

        // player images
        private Image playerTaxiThrustNone =
            new Image(Path.Combine("Assets", "Images", "Taxi_Thrust_None.png"));
        private Image playerTaxiThrustNoneRight =
            new Image(Path.Combine("Assets", "Images", "Taxi_Thrust_None_Right.png"));

        private IBaseImage playerTaxiThrustBack = new ImageStride(1000,
            ImageStride.CreateStrides(2, Path.Combine("Assets", "Images", "Taxi_Thrust_Back.png")));
        private IBaseImage playerTaxiThrustBackRight = new ImageStride(1000,
            ImageStride.CreateStrides(2, Path.Combine("Assets", "Images", "Taxi_Thrust_Back_Right.png")));

        private IBaseImage playerTaxiThrustBottom = new ImageStride(1000,
            ImageStride.CreateStrides(2, Path.Combine("Assets", "Images", "Taxi_Thrust_Bottom.png")));
        private IBaseImage playerTaxiThrustBottomBack = new ImageStride(1000,
            ImageStride.CreateStrides(2, Path.Combine("Assets", "Images", "Taxi_Thrust_Bottom_Back.png")));

        private IBaseImage playerTaxiThrustBottomRight = new ImageStride(1000,
            ImageStride.CreateStrides(2, Path.Combine("Assets", "Images", "Taxi_Thrust_Bottom_Right.png")));
        private IBaseImage playerTaxiThrustBottomBackRight = new ImageStride(1000,
            ImageStride.CreateStrides(2, Path.Combine("Assets", "Images", "Taxi_Thrust_Bottom_Back_Right.png")));

        // customer images
        private IBaseImage customerWalkingStridesLeft = new ImageStride(1000,
            ImageStride.CreateStrides(2, Path.Combine("Assets", "Images", "CustomerWalkLeft.png")));
        private IBaseImage customerWalkingStridesRight = new ImageStride(1000,
            ImageStride.CreateStrides(2, Path.Combine("Assets", "Images", "CustomerWalkRight.png")));

        private IBaseImage customerStandLeft= new Image(Path.Combine("Assets", "Images", "CustomerStandLeft.png"));
        private IBaseImage customerStandRight = new Image(Path.Combine("Assets", "Images", "CustomerStandRight.png"));

        // get player image
        public IBaseImage GetPlayerImage(TaxiOrientation orientation, TaxiBoosterState state) {
            switch (orientation) {
            case TaxiOrientation.TaxiOrientedLeft:
                switch (state) {
                case TaxiBoosterState.TaxiBoosterOff:
                    return playerTaxiThrustNone;
                case TaxiBoosterState.TaxiBoosterHorizontal:
                    return playerTaxiThrustBack;
                case TaxiBoosterState.TaxiBoosterUp:
                    return playerTaxiThrustBottom;
                case TaxiBoosterState.TaxiBoosterUpAndHorizontal:
                    return playerTaxiThrustBottomBack;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
                break;

            case TaxiOrientation.TaxiOrientedRight:
                switch (state) {
                case TaxiBoosterState.TaxiBoosterOff:
                    return playerTaxiThrustNoneRight;
                case TaxiBoosterState.TaxiBoosterHorizontal:
                    return playerTaxiThrustBackRight;
                case TaxiBoosterState.TaxiBoosterUp:
                    return playerTaxiThrustBottomRight;
                case TaxiBoosterState.TaxiBoosterUpAndHorizontal:
                    return playerTaxiThrustBottomBackRight;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }

        // get customer image
        public IBaseImage GetCustomerImage(CustomerOrientation orientation, CustomerState state) {
            switch (orientation) {
            case CustomerOrientation.CustomerOrientedLeft:
                switch (state) {
                    case CustomerState.CustomerStateStanding:
                        return customerStandLeft;
                    case CustomerState.CustomerStateWalking:
                        return customerWalkingStridesLeft;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }

            case CustomerOrientation.CustomerOrientedRight:
                switch (state) {
                case CustomerState.CustomerStateStanding:
                    return customerStandRight;
                case CustomerState.CustomerStateWalking:
                    return customerWalkingStridesRight;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }

            default:
                throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }
    }
}