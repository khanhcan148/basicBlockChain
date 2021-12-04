// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

var blockChain = new BlockChain(5);
blockChain.AddBlock(new {
    FirstName = "Khanh", LastName = "Tran"
});
blockChain.AddBlock(new {
    FirstName = "Khanh", LastName = "sdfsdf"
});
blockChain.AddBlock(new {
    FirstName = "Khanh", LastName = "Tran"
});
blockChain.AddBlock(new {
    FirstName = "Khanh", LastName = "Tran"
});
blockChain.AddBlock(new {
    FirstName = "Khanh", LastName = "Tran"
});
blockChain.AddBlock(new {
    FirstName = "Khanh", LastName = "Tran"
});

Console.WriteLine(blockChain.IsValid());
Console.WriteLine("END");



public class Block
{
    public string PreHash { get; private set; }
    public string Hash { get; private set; }
    public object Data { get; private set; }
    public DateTime CreatedDate { get; private set; }
    private int mineSalt { get; set; }

    public Block(string prevHash, object data)
    {
        this.PreHash = prevHash;
        this.Data = data;
        this.CreatedDate = DateTime.UtcNow;
        this.Hash = GenerateHash();
        this.mineSalt = 0;
    }

    public string GenerateHash()
    {
        using SHA256 mySHA256 = SHA256.Create();
        var hashData =
            mySHA256.ComputeHash(Encoding.UTF8.GetBytes(this.PreHash + JsonSerializer.Serialize(this.Data) + mineSalt +
                                                        this.CreatedDate.ToString()));
        StringBuilder builder = new StringBuilder();
        foreach (var t in hashData)
        {
            builder.Append(t.ToString("x2"));
        }

        return builder.ToString();
    }

    public void MineBlock(int difficulty)
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        var requireString = string.Concat(Enumerable.Repeat("0", difficulty));
        while (!this.Hash.StartsWith(requireString))
        {
            this.mineSalt++;
            this.Hash = GenerateHash();
        }
        stopWatch.Stop();
        Console.WriteLine($"Hash:{this.Hash} - PreHash: {this.PreHash} - CreatedDate: {this.CreatedDate} - Runtime: {stopWatch.Elapsed.TotalSeconds}");
    }
}

public class BlockChain
{
    private readonly int _difficulty;
    public List<Block> _blockChain  { get; private set; }

    public BlockChain(int difficulty)
    {
        _difficulty = difficulty;
        _blockChain = new List<Block>() { new Block("", new { isGenesisBlock = true }) };
    }

    private Block GetLatestBlock()
    {
        return _blockChain.Last();
    }

    public void AddBlock(object data)
    {
        var block = new Block(GetLatestBlock().Hash, data);
        block.MineBlock(_difficulty);
        this._blockChain.Add(block);
    }

    public bool IsValid()
    {
        for (int i = 1; i < _blockChain.Count; i++)
        {
            if (_blockChain[i].Hash != _blockChain[i].GenerateHash() ||
                _blockChain[i].PreHash != _blockChain[i - 1].Hash)
                return false;
        }
        return true;
    }
}