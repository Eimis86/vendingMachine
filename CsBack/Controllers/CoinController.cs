using CsBack.Model;
using Microsoft.AspNetCore.Mvc;

namespace CsBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoinController : ControllerBase
    {
        private static List<Coin> VendingMachineItems = new List<Coin>()
        {
            new Coin
            {
                Id = 1,
                Amount = 25,
                CoinType = "ct",
            },
            new Coin{
                Id=2,
                Amount = 60,
                CoinType = "ct",
            },
            new Coin{
                Id=3,
                Amount = 95,
                CoinType = "ct",
            },
            new Coin{
                Id=4,
                Amount = 5,
                CoinType = "ct",
            },
            new Coin{
                Id=5,
                Amount = 60,
                CoinType = "ct",
            },
            new Coin{
                Id=6,
                Amount = 30,
                CoinType = "ct",
            },
            new Coin{
                Id=7,
                Amount = 55,
                CoinType = "ct",
            },
            new Coin{
                Id=8,
                Amount = 85,
                CoinType = "ct",
            },
            new Coin{
                Id=9,
                Amount = 110,
                CoinType = "ct",
            },
        };

        private static List<VendingCoin> VendingMachineCoins = new List<VendingCoin>();

        [HttpGet("Items")]
        public async Task<ActionResult<List<Coin>>> getVendingItems()
        {       
            return Ok(VendingMachineItems);
        }

        [HttpGet("VendingCoins")]
        public async Task<ActionResult<List<VendingCoin>>> getVendingCoins()
        {
            return Ok(VendingMachineCoins);
        }

        [HttpPost("VendingCoins")]
        public async Task<ActionResult<List<VendingCoin>>> addVendingCoins(List<VendingCoin> vendingCoins)
        {
            foreach(var item in vendingCoins)
            {
            VendingMachineCoins.Add(item);
            }
            return Ok(VendingMachineCoins);
        }

        [HttpPut("UpdateVendingCoinAmount")]
        public async Task<ActionResult<List<VendingCoin>>> updateVendingCoinsAmount(VendingCoin vendingCoins)
        {
            var vendignCoin = VendingMachineCoins.Find(item =>  item.Coin == vendingCoins.Coin);
            if(vendignCoin == null)
            {
                return BadRequest();
            }
            vendignCoin.CoinAmount -= vendingCoins.CoinAmount;

            return Ok(VendingMachineCoins);
        }

        [HttpGet("userChange")]
        public async Task<ActionResult<List<VendingCoin>>> getUserChange(int itemId, int userMoney)
        {
            var item = VendingMachineItems.Find(item => item.Id == itemId);
            if (item == null)
            {
                return BadRequest();
            }
            var allUserChange = userMoney - item.Amount;

            if(item.Amount > userMoney)
            {
                return Ok("Need more money");
            }
            var UsedCoinList = new List<int>();
            var resultList = new List<VendingCoin>();
            await CalculateChange(UsedCoinList, resultList, allUserChange);

            return resultList;

        }

        private async Task<List<VendingCoin>> CalculateChange(List<int> UsedCoinList, List<VendingCoin> resultList, int allUserChange)
        {
            VendingCoin resultCoin = new VendingCoin();
            int Max = 0;
            int coinAmount = 0; 
            foreach (var itemCoin in VendingMachineCoins)
            {
                if (Max < itemCoin.Coin && itemCoin.CoinAmount != 0 && !UsedCoinList.Contains(itemCoin.Coin))
                {
                    coinAmount = itemCoin.CoinAmount;
                    Max = itemCoin.Coin;
                }
            }
            UsedCoinList.Add(Max);

            int i = 0;
            while (allUserChange >= Max && coinAmount != 0)
            {
                allUserChange -= Max;
                i++;
                resultCoin.Coin = Max;
                resultCoin.CoinAmount = i;
                coinAmount -= 1;

            }

            if(i > 0)
            {
                resultList.Add(resultCoin);
                await updateVendingCoinsAmount(resultCoin);
            }

            if (allUserChange > 0)
            {
                if(Max == 0)
                {
                    return null;
                }
                await CalculateChange(UsedCoinList, resultList, allUserChange);
            }

            return resultList;
        } 

    }
}
