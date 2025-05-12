using System;

namespace SimpleDatabaseConnectionTemplate.Models
{
	/// <summary>
	/// Base class for all database entities
	/// </summary>
	public abstract class EntityBase
	{
		/// <summary>
		/// Gets or sets the primary key identifier
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the creation date
		/// </summary>
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		/// <summary>
		/// Gets or sets the last update date
		/// </summary>
		public DateTime? UpdatedAt { get; set; }

		/// <summary>
		/// Gets the table name for this entity
		/// </summary>
		/// <returns>Table name</returns>
		public abstract string GetTableName();

		/// <summary>
		/// Validates the entity
		/// </summary>
		/// <returns>True if the entity is valid</returns>
		public virtual bool Validate()
		{
			return true;
		}
	}
}