﻿using WebApi.Models.Enum;
using WebApi.Models;
using WebApi.MyDbContext;

namespace WebApi.Sevice.Service
{
    public class WalletService
    {
        private readonly MyDb _myDb;

        public WalletService()
        {
        }

        public WalletService(MyDb myDb)
        {
            _myDb = myDb;
        }

        public Wallet CreateWalletForUser(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "User information is missing.");
                }

                // Tạo một cửa hàng mới
                Wallet newWallet = new Wallet
                {
                    UserId = user.Id, // Đặt UserId bằng ID của người dùng 
                };

                _myDb.Wallets.Add(newWallet);
                _myDb.SaveChanges();

                return newWallet;
            }
            catch (Exception e)
            {
                throw new Exception($"An error occurred: {e.Message}");
            }
        }
    }
}
