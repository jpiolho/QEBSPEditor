using Moq;
using QEBSPEditor.Models;
using QEBSPEditor.Models.BSPFiles;

namespace QEBSPEditor.Tests
{
    public class BSPEntitySourceEditorTests
    {
        #region ParseEntitiesFromSource
        [Fact]
        public void ParseEntitiesFromSource_SingleEntity()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.Setup(b => b.Entities).Returns(
@"{ 
    ""classname"" ""func_wall"" 
}");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);

            // Act
            editor.ParseEntitiesFromSource();

            // Assert
            Assert.NotEmpty(editor.Entities);
            Assert.Equal(1, editor.Entities.Count);
            Assert.Equal("func_wall", editor.Entities[0].Classname);
        }

        [Fact]
        public void ParseEntitiesFromSource_EmptySource()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.Setup(b => b.Entities).Returns("");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);

            // Act
            editor.ParseEntitiesFromSource();

            // Assert
            Assert.Empty(editor.Entities);
        }

        [Fact]
        public void ParseEntitiesFromSource_MultipleEntities()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.Setup(b => b.Entities).Returns(
        @"{ 
    ""classname"" ""func_wall"" 
    ""targetname"" ""mywall""
}
{ 
    ""classname"" ""func_door"" 
    ""targetname"" ""mydoor""
}");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);

            // Act
            editor.ParseEntitiesFromSource();

            // Assert
            Assert.Equal(2, editor.Entities.Count);
            Assert.Equal("func_wall", editor.Entities[0].Classname);
            Assert.Equal("mywall", editor.Entities[0].Targetname);
            Assert.Equal("func_door", editor.Entities[1].Classname);
            Assert.Equal("mydoor", editor.Entities[1].Targetname);
        }

        [Fact]
        public void ParseEntitiesFromSource_InvalidEntity()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.Setup(b => b.Entities).Returns(
@"{ 
    invalidkey ""func_wall"" 
}");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);

            // Act
            editor.ParseEntitiesFromSource();

            // Assert
            Assert.Empty(editor.Entities);
        }

        [Fact]
        public void ParseEntitiesFromSource_WithAdditionalKeys()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.Setup(b => b.Entities).Returns(
@"{ 
    ""classname"" ""func_wall""
    ""health"" ""100""
    ""targetname"" ""mywall""
}");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);

            // Act
            editor.ParseEntitiesFromSource();

            // Assert
            Assert.Equal(1, editor.Entities.Count);
            Assert.Equal("func_wall", editor.Entities[0].Classname);
            Assert.Equal("mywall", editor.Entities[0].Targetname);
            Assert.Equal("100", editor.Entities[0].GetKeyValue("health"));
        }

        [Fact]
        public void ParseEntitiesFromSource_Minified()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.Setup(b => b.Entities).Returns("{\"classname\"\"func_wall\"\"health\"\"100\"\"targetname\"\"mywall\"}");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);

            // Act
            editor.ParseEntitiesFromSource();

            // Assert
            Assert.Equal(1, editor.Entities.Count);
            Assert.Equal("func_wall", editor.Entities[0].Classname);
            Assert.Equal("mywall", editor.Entities[0].Targetname);
            Assert.Equal("100", editor.Entities[0].GetKeyValue("health"));
        }

        [Fact]
        public void ParseEntitiesFromSource_EntityNotClosed()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.Setup(b => b.Entities).Returns(
@"{ 
    ""classname"" ""func_wall"" 
{
    ""classname"" ""func_door""
}");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);

            // Act
            editor.ParseEntitiesFromSource();

            // Assert
            Assert.NotEmpty(editor.Entities);
            Assert.Equal(1, editor.Entities.Count);
            Assert.Equal("func_door", editor.Entities[0].Classname);
        }

        #endregion

        #region SetSource

        [Fact]
        public void SetSource_WithParseEntities()
        {
            // Arrange
            var source = "{ \"classname\" \"func_rotate\" }";
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.SetupProperty(b => b.Entities);

            var editor = new BSPEntitySourceEditor(mockBSP.Object);

            // Act
            editor.SetSource(source, true);

            // Assert
            Assert.Equal(source, mockBSP.Object.Entities);
            Assert.NotEmpty(editor.Entities);
        }

        [Fact]
        public void SetSource_WithoutParseEntities()
        {
            // Arrange
            var source = "{ \"classname\" \"item_ammo\" }";
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.SetupProperty(b => b.Entities);

            var editor = new BSPEntitySourceEditor(mockBSP.Object);

            // Act
            editor.SetSource(source, false);

            // Assert
            Assert.Equal(source, mockBSP.Object.Entities);
            Assert.Empty(editor.Entities);
        }

        #endregion

        #region AddEntity

        [Fact]
        public void AddEntity_NewEntityAdded()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.SetupProperty(b => b.Entities, "");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);

            // Act
            var newEntity = editor.AddEntity("func_wall");

            // Assert
            Assert.NotNull(newEntity);
            Assert.Equal("func_wall", newEntity.Classname);
            Assert.NotEmpty(editor.Entities);
            Assert.Equal(1, editor.Entities.Count);
        }

        [Fact]
        public void AddEntity_NewEntityAddedAndCanParseAgain()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.SetupProperty(b => b.Entities, "");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);

            // Act
            var newEntity = editor.AddEntity("func_wall");
            editor.ParseEntitiesFromSource();

            // Assert
            Assert.NotNull(newEntity);
            Assert.NotEmpty(editor.Entities);
            Assert.Equal(1, editor.Entities.Count);
            Assert.Equal("func_wall", editor.Entities[0].Classname);
        }

        [Fact]
        public void AddEntity_NewEntityAddedWithOtherEntitiesAlreadyPresent()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.SetupProperty(b => b.Entities,
@"{
    ""classname"" ""item_health""
    ""health""  ""100""
}");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);

            // Act
            editor.ParseEntitiesFromSource();
            var newEntity = editor.AddEntity("func_wall");

            // Assert
            Assert.NotEmpty(editor.Entities);
            Assert.Equal(2, editor.Entities.Count);
        }

        [Fact]
        public void AddEntity_NewEntityAddedWithOtherEntitiesAlreadyPresentAndCanParseAgain()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.SetupProperty(b => b.Entities,
@"{
    ""classname"" ""item_ammo""
    ""ammo""  ""25""
}");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);

            // Act
            editor.ParseEntitiesFromSource();
            var newEntity = editor.AddEntity("func_wall");
            editor.ParseEntitiesFromSource();

            // Assert
            Assert.NotNull(newEntity);
            Assert.NotEmpty(editor.Entities);
            Assert.Equal(2, editor.Entities.Count);
            Assert.Equal("item_ammo", editor.Entities[0].Classname);
            Assert.Equal("func_wall", editor.Entities[1].Classname);
        }

        [Fact]
        public void AddEntity_ClassNameSetCorrectly()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.SetupProperty(b => b.Entities, "");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);

            // Act
            var newEntity = editor.AddEntity("func_wall");

            // Assert
            Assert.Equal(1, editor.Entities.Count);
            Assert.Equal("func_wall", newEntity.Classname);
        }

        [Fact]
        public void AddEntity_SourceHintUpdated()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.SetupProperty(b => b.Entities, "");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);

            // Act
            var newEntity = editor.AddEntity("func_wall");

            // Assert
            Assert.NotNull(newEntity.SourceHint);
            Assert.Equal(0, newEntity.SourceHint.OffsetStart);
            Assert.True(newEntity.SourceHint.OffsetEnd > newEntity.SourceHint.OffsetStart);
        }

        #endregion

        #region RemoveEntity

        [Fact]
        public void RemoveEntity_EntityIsRemoved()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.SetupProperty(b => b.Entities, "");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);
            var entity = editor.AddEntity("func_wall");

            // Act
            editor.RemoveEntity(entity);

            // Assert
            Assert.Empty(editor.Entities);
        }


        [Fact]
        public void RemoveEntity_SourceIsUpdated()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.SetupProperty(b => b.Entities,
@"{
    ""classname"" ""func_wall""
}");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);
            editor.ParseEntitiesFromSource();

            // Act
            editor.RemoveEntity(editor.Entities[0]);

            // Assert
            Assert.True(string.IsNullOrEmpty(editor.Source));
        }

        [Fact]
        public void RemoveEntity_SourceWithNewLinesKeepsFormatting()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.SetupProperty(b => b.Entities,
@"{
    ""classname"" ""func_wall""
}
{
    ""classname"" ""weapon_rocketlauncher""
    ""ammo"" ""1234""
}
{
    ""classname"" ""item_shells""
}");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);
            editor.ParseEntitiesFromSource();

            // Act
            editor.RemoveEntity(editor.Entities[1]);

            // Assert
            Assert.Equal(@"{
    ""classname"" ""func_wall""
}
{
    ""classname"" ""item_shells""
}", editor.Source);
        }

        [Fact]
        public void RemoveEntity_SourceMinifiedKeepsFormatting()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.SetupProperty(b => b.Entities,@"{""classname""""func_wall""}{""classname""""weapon_rocketlauncher""""ammo""""1234""}{""classname""""item_shells""}");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);
            editor.ParseEntitiesFromSource();

            // Act
            editor.RemoveEntity(editor.Entities[1]);

            // Assert
            Assert.Equal(@"{""classname""""func_wall""}{""classname""""item_shells""}", editor.Source);
        }

        [Fact]
        public void RemoveEntity_SourceWithoutNewLinesKeepsFormatting()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.SetupProperty(b => b.Entities, @"{ ""classname"" ""func_wall"" } {""classname"" ""weapon_rocketlauncher"" ""ammo"" ""1234"" } { ""classname"" ""item_shells"" }");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);
            editor.ParseEntitiesFromSource();

            // Act
            editor.RemoveEntity(editor.Entities[1]);

            // Assert
            Assert.Equal(@"{ ""classname"" ""func_wall"" } { ""classname"" ""item_shells"" }", editor.Source);
        }

        [Fact]
        public void RemoveEntity_SourceIsUpdatedCorrectlyWhenMultipleEntitiesRemoved()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.SetupProperty(b => b.Entities,
@"{
    ""classname"" ""worldspawn""
    ""message"" ""my map""
}
{
    ""classname"" ""weapon_rocketlauncher""
    ""ammo"" ""1234""
    ""targetname"" ""rocket_launcher""
}
{
    ""classname"" ""item_shells""
}");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);
            editor.ParseEntitiesFromSource();

            // Act
            editor.RemoveEntity(editor.Entities[1]);
            editor.RemoveEntity(editor.Entities[0]);

            // Assert
            Assert.Equal(@"{
    ""classname"" ""item_shells""
}", editor.Source);
        }

        [Fact]
        public void RemoveEntity_RemoveMultipleEntitiesAndParseAgain()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.SetupProperty(b => b.Entities,
@"{
    ""classname"" ""worldspawn""
    ""message"" ""my map""
}
{
    ""classname"" ""light""
    ""targetname"" ""light1""
    ""health"" ""1000""
}
{
    ""classname"" ""info_player_start""
    ""origin"" ""123 123 123""
}
{
    ""classname"" ""weapon_rocketlauncher""
    ""origin"" ""54 -452 -342""
    ""ammo"" ""25""
}");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);
            editor.ParseEntitiesFromSource();

            // Act
            editor.RemoveEntity(editor.Entities[1]);
            editor.RemoveEntity(editor.Entities[1]);
            editor.ParseEntitiesFromSource();

            // Assert
            Assert.Equal(2, editor.Entities.Count);
            Assert.Equal("worldspawn", editor.Entities[0].Classname);
            Assert.Equal("my map", editor.Entities[0].GetKeyValue("message"));
            Assert.Equal("weapon_rocketlauncher", editor.Entities[1].Classname);
            Assert.Equal("25", editor.Entities[1].GetKeyValue("ammo"));
            Assert.Equal("54 -452 -342", editor.Entities[1].GetKeyValue("origin"));
        }

        [Fact]
        public void RemoveEntity_SourceHintAdjustedCorrectly()
        {
            // Arrange
            var mockBSP = new Mock<IBSPFile>();
            mockBSP.SetupProperty(b => b.Entities, "");

            var editor = new BSPEntitySourceEditor(mockBSP.Object);
            var entity1 = editor.AddEntity("func_wall");
            var entity2 = editor.AddEntity("func_door");

            var originalLength = entity2.SourceHint!.Length;

            // Act
            editor.RemoveEntity(entity1);

            // Assert
            Assert.NotNull(entity2.SourceHint);
            Assert.NotNull(entity1.SourceHint);
            Assert.Equal(entity2.SourceHint.OffsetStart, entity1.SourceHint.OffsetStart);
            Assert.Equal(entity2.SourceHint.Length, originalLength);
        }

        #endregion

        #region UpdateEntity

        [Fact]
        public void UpdateEntity_UpdatesSourceAndParsesItAgain()
        {
            // Arrange
            const string Message = "This is my wall";

            string source = @"{""classname"" ""func_wall""}";

            var mockBSP = new Mock<IBSPFile>();
            mockBSP.SetupProperty(b => b.Entities, source);
            
            var editor = new BSPEntitySourceEditor(mockBSP.Object);
            var editor2 = new BSPEntitySourceEditor(mockBSP.Object);

            // Act
            editor.ParseEntitiesFromSource();

            var sourceBefore = editor.Source;

            var entity = editor.Entities[0];
            entity.SetKeyValue("message", Message);
            editor.UpdateEntity(editor.Entities[0]);

            // - Now lets parse the source again using a separate editor
            source = editor.Source;
            editor2.ParseEntitiesFromSource();

            // Assert
            Assert.NotEmpty(editor2.Entities);
            Assert.Equal(1, editor2.Entities.Count);
            Assert.Equal("func_wall", editor2.Entities[0].Classname);
            Assert.Equal(Message, editor2.Entities[0].GetKeyValue("message"));
        }

        #endregion
    }
}