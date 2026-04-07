// *******************************************************************************************
// Copyright © 2019 Xiippy.ai. All rights reserved. Australian patents awarded. PCT patent pending.
//
// NOTES:
//
// - No payment gateway SDK function is consumed directly. Interfaces are defined out of such interactions and then the interface is implemented for payment gateways. Design the interface with the most common members and data structures between different gateways. 
// - A proper factory or provider must instantiate an instance of the interface that is interacted with.
// - Any major change made to SDKs should begin with the c sharp SDK with the mindset to keep the high-level syntax, structures and class names the same to minimise porting efforts to other languages. Do not use language specific features that do not exist in other languages. We are not in the business of doing the same thing from scratch multiple times in different forms.
// - Pascal Case for naming conventions should be used for all languages
// - No secret or passwords or keys must exist in the code when checked in
//
// *******************************************************************************************

using Org.BouncyCastle.Math.EC.Rfc8032;

namespace Xiippy.NEXOTerminalSDK.Utils
{
    public class RequestSignatureHandlerClientSide
    {
        /// <summary>
        /// Generates a detached signature for an HTTP request by combining the request body and timestamp.
        /// The signature is created using Ed25519 private key cryptography.
        /// </summary>
        /// <param name="body">The byte array containing the HTTP request body</param>
        /// <param name="momentInMilliseconds">The current time in milliseconds (Unix timestamp)</param>
        /// <param name="privateKey">The Ed25519 private key used for signing</param>
        /// <returns>A byte array containing the detached signature</returns>
        public static byte[] GenerateSignatureForRequest(byte[] body, long momentInMilliseconds, byte[] privateKey)
        {
            string moment = momentInMilliseconds.ToString();
            byte[] momentBytes = System.Text.Encoding.UTF8.GetBytes(moment);

            byte[] dataToSign = CombineBodyAndMoment(body, momentBytes);

            return SignDetached(dataToSign, privateKey);
        }

      

        /// <summary>
        /// Combines the request body and moment (timestamp) using the format: {body}#{moment}
        /// </summary>
        /// <param name="body">The byte array containing the request body</param>
        /// <param name="momentBytes">The byte array containing the timestamp</param>
        /// <returns>A combined byte array in the format body#moment</returns>
        public static byte[] CombineBodyAndMoment(byte[] body, byte[] momentBytes)
        {
            byte[] separator = System.Text.Encoding.UTF8.GetBytes("#");

            byte[] combined = new byte[body.Length + separator.Length + momentBytes.Length];
            System.Buffer.BlockCopy(body, 0, combined, 0, body.Length);
            System.Buffer.BlockCopy(separator, 0, combined, body.Length, separator.Length);
            System.Buffer.BlockCopy(momentBytes, 0, combined, body.Length + separator.Length, momentBytes.Length);

            return combined;
        }





        /// <summary>
        /// Signs the message using a private ED25519 key
        /// </summary>
        /// <param name="message"></param>
        /// <param name="PrivateKey"></param>
        /// <returns></returns>
        public static byte[] SignDetached(byte[] message, byte[] PrivateKey)
        {
            byte[] signature = new byte[Ed25519.SignatureSize];
            Ed25519.Sign(PrivateKey, 0, message, 0, message.Length, signature, 0);
            return signature;
        }



    }
}
